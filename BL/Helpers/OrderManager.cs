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
        internal static void AddOrder(int applicantId, BO.Order boOrder)
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

            
    
        
           

        internal static int CalculateAirDistance(double latitude, double longitude)
        {
            return 50;
        }

        /// This method is not permitted to delete orders and always throws a logical exception according to system requirements.
        internal static void DeleteOrder(int applicantId, int orderId)
        {
            throw new BlUnauthorizedAccessException(
                "Orders cannot be deleted in the system.");
        }       



        // Retrieves full order details and converts them to a business object
        internal static BO.Order GetOrderDetails(int applicantId, int orderId)
        {
            DO.Order doOrder = s_dal.Order.Read(orderId);

            return new BO.Order
            {
                ID = doOrder.Id,
                OrderType = (BO.OrderType)doOrder.OrderType,
                VerbalDescription = doOrder.OrderNote,
                FullAddressOfTheOrder = doOrder.CustomerAddress,
                Latitude = doOrder.Latitude,
                Longitude = doOrder.Longitude,
                FullNameOfTheInviter = doOrder.CustomerFullName,
                OrderersPhoneNumber = doOrder.CustomerPhone,
                OrderOpenDate = doOrder.OrderDate
                // Logical fields (status, timing, deliveries) are calculated elsewhere
            };
        }

        // Returns a list of orders formatted for list display
        internal static IEnumerable<OrderInList> GetOrderList(
            int applicantId,
            filterOrdersByProperty? filterBy,
            object? type,
            sortOrdersByProperty? sortBy)
        {
            return s_dal.Order.ReadAll()
                .Select(o => new OrderInList
                {
                    OrderId = o.Id,
                    DeliveryType = BO.DeliveryType.None,
                    AirDistance = 0,
                    OrderStatus = OrderStatus.Open,
                    ScheduleStatus = ScheduleStatus.OnTime,
                    DeliveryTimeLeft = TimeSpan.Zero,
                    TotalHandlingTime = null,
                    TotalDeliveries = 0
                });
        }



        // Returns the total count of orders grouped by logical status
        internal static IEnumerable<int> GetOrdersStatusCounts(int applicantId)
        {
            // Since order status is not stored in DO.Order,
            // the count is currently calculated logically
            return new List<int> { s_dal.Order.ReadAll().Count() };
        }

        /// Helper method to convert BO.Order to DO.Order
        internal static DO.Order ConvertToOrder(BO.Order order)
        {
            DO.Order newOrder = s_dal.Order.Read(order.ID);
            return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.ID} does not exist.");
        }
        
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        internal static BO.Order ConvertToOrder(DO.Order order)
        {

            BO.Order boOrder = new BO.Order
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
                deliveryPerOrderList = DeliveryManager.GetDeliveryPerOrderList(order.Id) ?? null,
                OrderProperties = (BO.OrderProperties)order.OrderProperties
            };



            // set order status and timing based on deliveries
            if (boOrder.deliveryPerOrderList.Count()==0)
            {
                boOrder.OrderStatus = OrderStatus.Open;
                // ts = Time elapsed since the order was opened
                TimeSpan ts = AdminManager.Now - boOrder.OrderOpenDate;
                boOrder.TimeLeftToCompleteOrder = AdminManager.MaxDeliveryDuration - ts;
                if (boOrder.TimeLeftToCompleteOrder > AdminManager.DelayRiskTime)
                {
                    boOrder.ScheduleStatus = ScheduleStatus.OnTime;
                }
                else if (boOrder.TimeLeftToCompleteOrder< AdminManager.DelayRiskTime)
                {
                    boOrder.ScheduleStatus = ScheduleStatus.InRisk;
                }
                else
                {
                    if (TimeSpan.Zero > boOrder.TimeLeftToCompleteOrder)
                        boOrder.ScheduleStatus = ScheduleStatus.Late;
                }
            }
            else
            {
                DO.Delivery? delivery = DeliveryManager.GetLastDelivery(boOrder.deliveryPerOrderList);
                if (delivery is not null)
                {
                    boOrder.TimeLeftToCompleteOrder = TimeSpan.Zero;
                    if (delivery.DeliveryTermintionType == DO.DeliveryTermintionType.Cancelled)
                        boOrder.OrderStatus = OrderStatus.Cancelled;
                    else if (delivery.DeliveryTermintionType == DO.DeliveryTermintionType.RefusedToAccept)
                        boOrder.OrderStatus = OrderStatus.Refused;
                    else if (delivery.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully)
                        boOrder.OrderStatus = OrderStatus.Delivered;
                        else if (delivery.DeliveryEndTime is null && delivery.DeliveryTermintionType == DO.DeliveryTermintionType.None)
                        {
                            boOrder.OrderStatus = OrderStatus.InTreatment;
                            boOrder.TimeLeftToCompleteOrder =
                                AdminManager.MaxDeliveryDuration - (AdminManager.Now - delivery.DeliveryStartTime);

                        }

                    if(boOrder.OrderStatus != OrderStatus.InTreatment)
                    {
                        if(delivery.DeliveryEndTime <= delivery.DeliveryStartTime+AdminManager.MaxDeliveryDuration)
                             boOrder.ScheduleStatus = ScheduleStatus.OnTime;
                        else
                        {
                             boOrder.ScheduleStatus = ScheduleStatus.Late;  
                        }

                    }
   
                }

                }
                return boOrder; 
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
        internal static IEnumerable<OrderInList> GetOrderListInternal( //מטודת עזר ל -  פונקצית GetOrderList
        int applicantId,
        filterOrdersByProperty? filterBy,
        object? type,
        sortOrdersByProperty? sortBy)
            {
                //  שליפת כל ההזמנות מה-DAL
                IEnumerable<DO.Order> doOrders = s_dal.Order.ReadAll();
                //  המרה ל-BO.Order (כולל חישוב סטטוסים ומשלוח אחרון)
                IEnumerable<BO.Order> boOrders = doOrders
                    .Select(o => ConvertToOrder(o));
                // סינון (רק אם נדרש)
                if (filterBy != null && type != null)
                {
                    switch (filterBy)
                    {
                        case filterOrdersByProperty.OrderStatus:
                            boOrders = boOrders.Where(o => o.OrderStatus == (BO.OrderStatus)type);
                            break;

                        case filterOrdersByProperty.OrderType:
                            boOrders = boOrders.Where(o => o.OrderType == (BO.OrderType)type);
                            break;

                        default:
                            // לא מסננים
                            break;
                    }

                }



                //  מיון – ברירת מחדל: לפי סטטוס הזמנה
                boOrders = sortBy switch
                {
                    // ברירת מחדל – מיון לפי סטטוס הזמנה
                    null =>
                        boOrders.OrderBy(o => o.OrderStatus),

                    sortOrdersByProperty.OrderDate =>
                        boOrders.OrderBy(o => o.OrderOpenDate),

                    sortOrdersByProperty.DeliveryDate =>
                        boOrders.OrderBy(o => o.EstimatedDeliveryTime),

                    sortOrdersByProperty.CustomerName =>
                        boOrders.OrderBy(o => o.FullNameOfTheInviter),

                    sortOrdersByProperty.OrderStatus =>
                        boOrders.OrderBy(o => o.OrderStatus),

                    _ => boOrders
                };


                //  המרה ל-OrderInList (ישות לוגית למסך)
                return boOrders.Select(o => new OrderInList
                {
                    OrderId = o.ID,

                    // משלוח אחרון (אם קיים)
                    DeliveryId = o.deliveryPerOrderList?.LastOrDefault()?.DeliveryId ?? 0,

                    DeliveryType = o.deliveryPerOrderList?.LastOrDefault()?.DeliveryType
                       ?? BO.DeliveryType.None,

                    AirDistance = o.AirDistance,

                    OrderStatus = o.OrderStatus,
                    ScheduleStatus = o.ScheduleStatus,

                    DeliveryTimeLeft = o.TimeLeftToCompleteOrder,

                    TotalHandlingTime = null,   // ← תיקון כאן

                    TotalDeliveries = o.deliveryPerOrderList?.Count ?? 0
                });
            }
        internal static BO.Order GetOrderDetails(int orderId) //פונקציית עזר - לפונקציה GetDetails
        {
            DO.Order? doOrder = s_dal.Order.Read(orderId);
            return ConvertToOrder(doOrder);
        }

        internal static void UpdateOrderDetails(BO.Order boOrder)// פונקציית עזר לפונקצייה UpdateDetails
        {
            DO.Order? oldOrder = s_dal.Order.Read(boOrder.ID);

            DO.Order? updatedOrder = oldOrder with
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

            s_dal.Order.Update(updatedOrder);
            Observers.NotifyItemUpdated(boOrder.ID);//stage 5
            Observers.NotifyListUpdated();//stage 5

        }


    }
}
