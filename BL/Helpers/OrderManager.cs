using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
namespace Helpers
{
    internal static class OrderManager
    {
        private static DalApi.IDal s_dal = DalApi.Factory.Get;

        internal static ObserverManager Observers = new(); //stage 5


        #region Create / Update / Delete
        internal static void AddOrder( BO.Order boOrder)
        {
            if (boOrder == null)
                throw new BlArgumentNullException(nameof(boOrder));

            DO.Order doOrder = new DO.Order(
                Id: boOrder.ID,
                OrderType: (DO.OrderType)boOrder.OrderType,
                OrderNote: boOrder.VerbalDescription ?? string.Empty,
                CustomerAddress: boOrder.FullAddressOfTheOrder ?? "",
                Latitude: boOrder.Latitude,
                Longitude: boOrder.Longitude,
                CustomerFullName: boOrder.FullNameOfTheInviter ?? "",
                CustomerPhone: boOrder.OrderersPhoneNumber ?? "",
                OrderDate: boOrder.OrderOpenDate,
                OrderProperties: (DO.OrderProperties)boOrder.OrderProperties
            );
            try
            {
                s_dal.Order.Create(doOrder);
            }
            catch (DO.DalXMLFileLoadCreateException ex)
            {
                // Provide clear description that creation failed due to XML/file issues in DAL
                throw new BlDataAccessException("Failed to create new order: data layer XML/file load or create error.", ex);
            }
            catch (DalAlreadyExistsException ex)
            {
                // Provide context about the conflicting order to help debugging
                string context = $"Order already exists '{doOrder.Id}'.";
                throw new BlAlreadyExistsException(context, ex);
            }
            Observers.NotifyListUpdated(); //stage 5

        }
        internal static void UpdateOrder(BO.Order boOrder)// פונקציית עזר לפונקצייה UpdateDetails
        {
            DO.Order oldOrder = s_dal.Order.Read(boOrder.ID);

            DO.Order updated = oldOrder with
            {
                OrderType = (DO.OrderType)boOrder.OrderType,
                OrderNote = boOrder.VerbalDescription ?? oldOrder.OrderNote,
                Latitude = boOrder.Latitude,
                Longitude = boOrder.Longitude,
                CustomerAddress = boOrder.FullAddressOfTheOrder ?? oldOrder.CustomerAddress,
                CustomerFullName = boOrder.FullNameOfTheInviter ?? oldOrder.CustomerFullName,
                CustomerPhone = boOrder.OrderersPhoneNumber ?? oldOrder.CustomerPhone,
                OrderProperties = (DO.OrderProperties)boOrder.OrderProperties,
                OrderDate = boOrder.OrderOpenDate
            };

            s_dal.Order.Update(updated);
            Observers.NotifyItemUpdated(boOrder.ID);//stage 5
            Observers.NotifyListUpdated();//stage 5

        }
        internal static void DeleteOrder( int orderId)
        {
            try
            {
                s_dal.Order.Delete(orderId);
            }
            catch(DalDoesNotExistException)
            {
                throw new BlDoesNotExistException($"Order {orderId} does not exist");
            }
        }

        #endregion

        #region Get Details
        internal static BO.Order GetOrderDetails(int orderId) //פונקציית עזר - לפונקציה GetDetails
        {
            DO.Order? doOrder = s_dal.Order.Read(orderId)
                ?? throw new BlDoesNotExistException($"Order with ID {orderId} does not exist.");
            return ConvertToOrder((DO.Order)doOrder);
        }

        #endregion
        internal static int CalculateAirDistance(double latitude, double longitude)
        {
            throw new Exception("Not implemented yet");
        }

        /// This method is not permitted to delete orders and always throws a logical exception according to system requirements.

        #region Lists & Counts
        internal static IEnumerable<OrderInList> GetOrderList(
            int applicantId,
            filterOrdersByProperty? filterBy,
            object? filterValue,
            sortOrdersByProperty? sortBy)
        {
            IEnumerable<BO.Order> orders =
                s_dal.Order.ReadAll().Select(ConvertToOrder);

            if (filterBy is not null && filterValue is not null)
            {
                orders = filterBy switch
                {
                    filterOrdersByProperty.OrderStatus =>
                        orders.Where(o => o.OrderStatus == (OrderStatus)filterValue),

                    filterOrdersByProperty.OrderType =>
                        orders.Where(o => o.OrderType == (BO.OrderType)filterValue),

                    _ => orders
                };
            }

            orders = sortBy switch
            {
                sortOrdersByProperty.OrderDate => orders.OrderBy(o => o.OrderOpenDate),
                sortOrdersByProperty.CustomerName => orders.OrderBy(o => o.FullNameOfTheInviter),
                sortOrdersByProperty.OrderStatus => orders.OrderBy(o => o.OrderStatus),
                _ => orders
            };

            return orders.Select(o => new OrderInList
            {
                OrderId = o.ID,
                DeliveryId = o.deliveryPerOrderList?.LastOrDefault()?.DeliveryId ?? 0,
                DeliveryType = o.deliveryPerOrderList?.LastOrDefault()?.DeliveryType ?? BO.DeliveryType.None,
                AirDistance = o.AirDistance,
                OrderStatus = o.OrderStatus,
                ScheduleStatus = o.ScheduleStatus,
                DeliveryTimeLeft = o.TimeLeftToCompleteOrder,
                TotalDeliveries = o.deliveryPerOrderList?.Count ?? 0
            });
        }




        #endregion

        #region Conversion

        internal static BO.Order ConvertToOrder(DO.Order order)
        {
            BO.Order bo = new()
            {
                ID = order.Id,
                OrderType = (BO.OrderType)order.OrderType,
                VerbalDescription = order.OrderNote,
                FullAddressOfTheOrder = order.CustomerAddress,
                Latitude = order.Latitude,
                Longitude = order.Longitude,
                FullNameOfTheInviter = order.CustomerFullName,
                OrderersPhoneNumber = order.CustomerPhone,
                OrderOpenDate = order.OrderDate,
                OrderProperties = (BO.OrderProperties)order.OrderProperties,
                deliveryPerOrderList = DeliveryManager.GetDeliveryPerOrderList(order.Id),
                MaximumDeliveryDate = AdminManager.Now + AdminManager.MaxDeliveryDuration
            };

            CalculateStatusAndTiming(bo);
            return bo;
        }

        private static void CalculateStatusAndTiming(BO.Order order)
        {
            var deliveries = order.deliveryPerOrderList;

            if (deliveries is null || !deliveries.Any())
            {
                order.OrderStatus = OrderStatus.Open;
                TimeSpan passed = AdminManager.Now - order.OrderOpenDate;
                order.TimeLeftToCompleteOrder = AdminManager.MaxDeliveryDuration - passed;

                order.ScheduleStatus =
                    order.TimeLeftToCompleteOrder <= TimeSpan.Zero ? ScheduleStatus.Late :
                    order.TimeLeftToCompleteOrder <= AdminManager.DelayRiskTime ? ScheduleStatus.InRisk :
                    ScheduleStatus.OnTime;

                return;
            }

            BO.DeliveryPerOrderInList last = deliveries.Last();

            if (last.DeliveryEndTime is null)
            {
                order.OrderStatus = OrderStatus.InTreatment;
                order.TimeLeftToCompleteOrder =
                    AdminManager.MaxDeliveryDuration - (AdminManager.Now - last.DeliveryStart);
                return;
            }

            order.TimeLeftToCompleteOrder = TimeSpan.Zero;

            order.OrderStatus = (DO.DeliveryTermintionType)last.DeliveryTerminationType switch
            {
                DO.DeliveryTermintionType.Cancelled => OrderStatus.Cancelled,
                DO.DeliveryTermintionType.RefusedToAccept => OrderStatus.Refused,
                DO.DeliveryTermintionType.DeliveredSeccessfully => OrderStatus.Delivered,
                _ => throw new BlInvalidStatusException("Unknown delivery termination type")
            };

            order.ScheduleStatus =
                last.DeliveryEndTime <= last.DeliveryStart + AdminManager.MaxDeliveryDuration
                ? ScheduleStatus.OnTime
                : ScheduleStatus.Late;
        }

        #endregion

        // Returns the total count of orders grouped by logical status
        internal static IEnumerable<int> GetOrdersStatusCounts(int applicantId)
        {
            // Since order status is not stored in DO.Order,
            // the count is currently calculated logically
            return new List<int> { s_dal.Order.ReadAll().Count() };
        }

        /// Helper method to convert BO.Order to DO.Order
        //internal static DO.Order ConvertToOrder(BO.Order order)
        //{
        //    DO.Order newOrder = s_dal.Order.Read(order.ID);
        //    return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.ID} does not exist.");
        //}
        
        /// <summary>
        /// Cancels the specified order if it is in a cancellable state.    
        /// </summary>
        /// <remarks>This method cancels an order only if its status is <see cref="OrderStatus.Open"/> or
        /// <see cref="OrderStatus.InTreatment"/>. If the order is open, a cancellation delivery record is created. If
        /// the order is in treatment, the active delivery is updated to reflect the cancellation. Observers are
        /// notified of the update after the operation completes.</remarks>
        /// <param name="orderId">The unique identifier of the order to cancel.</param>
        /// <exception cref="BO.BlInvalidStatusException">Thrown if the order is not in a state that allows cancellation.</exception>
        internal static void CancelOrder(int orderId)//מטודת עזר ל cancelOrder
        {
            //  קריאת ההזמנה
            DO.Order doOrder = s_dal.Order.Read(orderId);

            //  המרה ל-BO כדי לדעת סטטוס לוגי
            BO.Order boOrder = ConvertToOrder(doOrder);

            //  בדיקת חוקיות
            if (boOrder.OrderStatus != OrderStatus.Open &&
                boOrder.OrderStatus != OrderStatus.InTreatment)
            {
                throw new BO.BlInvalidStatusException(
                    $"Order {orderId} cannot be cancelled in status {boOrder.OrderStatus}");
            }

            DateTime now = DateTime.Now;

            //  אם ההזמנה פתוחה – יצירת משלוח מדומה
            if (boOrder.OrderStatus == OrderStatus.Open)
            {
                DO.Delivery fakeDelivery = new DO.Delivery
                {
                    Id = 0,
                    OrderId = orderId,
                    CourierId = 0,
                    DeliveryStartTime = now,
                    DeliveryEndTime = now,
                    DeliveryTermintionType = DO.DeliveryTermintionType.Cancelled,
                    ActualDistance = 0
                };

                s_dal.Delivery.Create(fakeDelivery);

            }

            //  אם ההזמנה בטיפול – עדכון משלוח קיים
            if (boOrder.OrderStatus == OrderStatus.InTreatment)
            {
                // מציאת המשלוח הפעיל
                DO.Delivery delivery = s_dal.Delivery.ReadAll()
                    .First(d => d.OrderId == orderId &&
                                d.DeliveryEndTime == null);
                DO.Delivery updatedDelivery = delivery with
                {
                    DeliveryEndTime = now,
                    DeliveryTermintionType = DO.DeliveryTermintionType.Cancelled
                };

                s_dal.Delivery.Update(updatedDelivery);

            }
            Observers.NotifyItemUpdated(orderId);//stage 5
            Observers.NotifyListUpdated();//stage 5

        }

        internal static IEnumerable<int> GetOrdersStatusCountsInternal(int applicantId)//פונקצית עזר לפונקציה GetOrdersStatusCounts 
        {
            int[] result = new int[9];

            IEnumerable<DO.Order> doOrders = s_dal.Order.ReadAll();

            IEnumerable<BO.Order> ordersOfApplicant = doOrders
                .Select(o => ConvertToOrder(o))
                .Where(o => o.ID == applicantId);

            var groupedByStatus = ordersOfApplicant
                .GroupBy(o => GetStatusIndex(o));

            foreach (var group in groupedByStatus)
            {
                result[group.Key] = group.Count();
            }

            return result;
        }

        private static int GetStatusIndex(BO.Order order)//פונקצית עזר לפונקציה GetOrdersStatusCounts היא מחזירה את האינדקס שבו צריך להיות ההזמנה על פי הסטטוס הזמנה והסטטוס זמן
        {
            // סטטוסים פעילים – תלויי זמן
            if (order.OrderStatus == OrderStatus.Open)
            {
                if (order.ScheduleStatus == ScheduleStatus.OnTime) return 0;
                if (order.ScheduleStatus == ScheduleStatus.InRisk) return 1;
                if (order.ScheduleStatus == ScheduleStatus.Late) return 2;
            }

            if (order.OrderStatus == OrderStatus.InTreatment)
            {
                if (order.ScheduleStatus == ScheduleStatus.OnTime) return 3;
                if (order.ScheduleStatus == ScheduleStatus.InRisk) return 4;
                if (order.ScheduleStatus == ScheduleStatus.Late) return 5;
            }

            // סטטוסים סופיים – לא תלויי זמן
            if (order.OrderStatus == OrderStatus.Delivered) return 6;
            if (order.OrderStatus == OrderStatus.Refused) return 7;
            if (order.OrderStatus == OrderStatus.Cancelled) return 8;

            throw new BO.BlInvalidStatusException(   //זריקת חריגה במידה ויש סטטוס לא מזוהה וערך לא תקין
                $"Invalid status combination: OrderStatus={order.OrderStatus}, ScheduleStatus={order.ScheduleStatus}");

        }
      

       


    }
}
