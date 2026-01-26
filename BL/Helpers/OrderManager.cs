using BlApi;
using BO;
using DalApi;
using DO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Helpers
{
    internal static class OrderManager
    {
        private static DalApi.IDal s_dal = DalApi.Factory.Get;

        internal static ObserverManager Observers = new(); //stage 5


        #region Create / Update / Delete
        internal static async Task AddOrderAsync(BO.Order boOrder)
        {
            if (boOrder == null)
                throw new BlArgumentNullException(nameof(boOrder));
            // חישוב קואורדינטות רק בעת הוספת הזמנה
            if ((boOrder.Latitude == 0 || boOrder.Longitude == 0) &&
                !string.IsNullOrWhiteSpace(boOrder.FullAddressOfTheOrder))
            {
                bool isValid =
                    await Tools.IsValidAddressAsync(boOrder.FullAddressOfTheOrder);

                if (!isValid)
                    throw new BO.BlDoesNotExistException("Invalid order address");

                var (lat, lon) =  await Tools.GetCoordinatesAsync(boOrder.FullAddressOfTheOrder);

                boOrder.Latitude = lat;
                boOrder.Longitude = lon;
            }

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
                lock (AdminManager.BlMutex)
                    s_dal.Order.Create(doOrder);
                
                Observers.NotifyListUpdated(); //stage 5
                
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
            

        }
        internal static void UpdateOrder(BO.Order boOrder)
        {
            DO.Order oldOrder;
            DO.Order updated;

            lock (AdminManager.BlMutex) // stage 7
            {
                // קריאה מה-DAL
                oldOrder = s_dal.Order.Read(boOrder.ID);

                // יצירת אובייקט מעודכן
                updated = oldOrder with
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

                // עדכון ב-DAL
                s_dal.Order.Update(updated);
            }

            // Notifications מחוץ ל-lock
            Observers.NotifyItemUpdated(boOrder.ID); // stage 5
            Observers.NotifyListUpdated();          // stage 5
        }

        internal static void DeleteOrder(int orderId)
        {
            try
            {
                lock (AdminManager.BlMutex) // stage 7
                    s_dal.Order.Delete(orderId);

                // Notifications מחוץ ל-lock
                Observers.NotifyItemUpdated(orderId); // stage 5
                Observers.NotifyListUpdated();        // stage 5
            }
            catch (DalDoesNotExistException)
            {
                throw new BlDoesNotExistException($"Order {orderId} does not exist");
            }
        }

        internal static IEnumerable<DO.Order> ReadAll()
        {
            lock (AdminManager.BlMutex) // stage 7
                return s_dal.Order.ReadAll().ToList();
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
        internal static void CancelOrder(int orderId)
        {
            DO.Order doOrder;
            BO.Order boOrder;
            DateTime now;

            lock (AdminManager.BlMutex) // stage 7
            {
                // קריאת ההזמנה
                doOrder = s_dal.Order.Read(orderId);

                // המרה ל-BO כדי לדעת סטטוס לוגי
                boOrder = ConvertToOrder(doOrder);

                // בדיקת חוקיות
                if (boOrder.OrderStatus == OrderStatus.Delivered
                    || boOrder.OrderStatus == OrderStatus.Refused
                    || boOrder.OrderStatus == OrderStatus.Cancelled)
                {
                    throw new BO.BlInvalidStatusException(
                        $"Order {orderId} cannot be cancelled in status {boOrder.OrderStatus}");
                }

                // זמן מערכת
                now = s_dal.Config.Clock;

                // אם ההזמנה פתוחה – יצירת משלוח מדומה
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

                // אם ההזמנה בטיפול – עדכון משלוח קיים
                if (boOrder.OrderStatus == OrderStatus.InTreatment)
                {
                    DO.Delivery delivery = DeliveryManager.GetLastDelivery(orderId);

                    if (delivery != null)
                    {
                        DO.Delivery updatedDelivery = delivery with
                        {
                            DeliveryEndTime = now,
                            DeliveryTermintionType = DO.DeliveryTermintionType.Cancelled
                        };

                        s_dal.Delivery.Update(updatedDelivery);
                    }
                }

                // עדכון סטטוס הזמנה
                boOrder.OrderStatus = OrderStatus.Cancelled;
                UpdateOrder(boOrder);
            }

            // Notifications מחוץ ל-lock
            Observers.NotifyItemUpdated(orderId); // stage 5
            Observers.NotifyListUpdated();        // stage 5
        }

        #endregion

        #region Get Details

        internal static BO.Order GetOrderDetails(int orderId)
        {
            DO.Order? doOrder;

            lock (AdminManager.BlMutex) // stage 7
                doOrder = s_dal.Order.Read(orderId);

            if (doOrder is null)
                throw new BlDoesNotExistException(
                    $"Order with ID {orderId} does not exist.");

            return ConvertToOrder(doOrder);
        }

        #endregion



        #region Lists & Counts


        internal static IEnumerable<OrderInList> GetOrderList(
            int applicantId,
            object? filterBy,
            object? filterValue,
            sortOrdersByProperty? sortBy)
        {
            List<DO.Order> doOrders;

            lock (AdminManager.BlMutex) // stage 7
                doOrders = s_dal.Order.ReadAll().ToList();

            IEnumerable<BO.Order> orders =
                doOrders.Select(ConvertToOrder);


            if (filterBy is not null && filterValue is not null && 
                filterValue  is not OrderStatus.All)
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

        // Returns the total count of orders grouped by logical status
        internal static IEnumerable<int> GetOrdersStatusCounts(int applicantId)
        {
            // Since order status is not stored in DO.Order,
            // the count is currently calculated logically
            List<DO.Order> list;

            lock (AdminManager.BlMutex) // stage 7
                list = s_dal.Order.ReadAll().ToList();

            return new List<int> { list.Count };
        }
        internal static int[] GetOrdersStatusCountsInternal(int applicantId)//פונקצית עזר לפונקציה GetOrdersStatusCounts 
        {
            int[] summary = new int[Enum.GetValues(typeof(OrderStatus)).Length];


            List<DO.Order> list;

            lock (AdminManager.BlMutex) // stage 7
                list = s_dal.Order.ReadAll().ToList();

            var result = list
                .Select(o => ConvertToOrder(o))
                .GroupBy(o => o.OrderStatus)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();



            foreach (var item in result)
            {
                summary[(int)item.Status] = item.Count;
            }
             return summary;

        }

        //private static int GetStatusIndex(BO.Order order)//פונקצית עזר לפונקציה GetOrdersStatusCounts היא מחזירה את האינדקס שבו צריך להיות ההזמנה על פי הסטטוס הזמנה והסטטוס זמן
        //{
        //    // סטטוסים פעילים – תלויי זמן
        //    if (order.OrderStatus == OrderStatus.Open)
        //    {
        //        if (order.ScheduleStatus == ScheduleStatus.OnTime) return 0;
        //        if (order.ScheduleStatus == ScheduleStatus.InRisk) return 1;
        //        if (order.ScheduleStatus == ScheduleStatus.Late) return 2;
        //    }

        //    if (order.OrderStatus == OrderStatus.InTreatment)
        //    {
        //        if (order.ScheduleStatus == ScheduleStatus.OnTime) return 3;
        //        if (order.ScheduleStatus == ScheduleStatus.InRisk) return 4;
        //        if (order.ScheduleStatus == ScheduleStatus.Late) return 5;
        //    }

        //    // סטטוסים סופיים – לא תלויי זמן
        //    if (order.OrderStatus == OrderStatus.Delivered) return 6;
        //    if (order.OrderStatus == OrderStatus.Refused) return 7;
        //    if (order.OrderStatus == OrderStatus.Cancelled) return 8;

        //    throw new BO.BlInvalidStatusException(   //זריקת חריגה במידה ויש סטטוס לא מזוהה וערך לא תקין
        //        $"Invalid status combination: OrderStatus={order.OrderStatus}, ScheduleStatus={order.ScheduleStatus}");

        //}



        #endregion



        #region Conversion

        internal static BO.Order ConvertToOrder(DO.Order order)
        {
            var config = AdminManager.GetConfig(); // צילום Config

            BO.Order bo;

            var currentDelivery = DeliveryManager.GetLastDelivery(order.Id);

            var maxDeliveryDate = order.OrderDate + config.MaxDeliveryDuration;

            if (currentDelivery is null)
            {
                // NO delivery with this Order ID.
                bo = new()
                {
                    ID = order.Id,
                    OrderType = (BO.OrderType)order.OrderType,
                    VerbalDescription = order.OrderNote,
                    FullAddressOfTheOrder = order.CustomerAddress,
                    Latitude = order.Latitude,
                    Longitude = order.Longitude,
                    AirDistance = 0,
                    FullNameOfTheInviter = order.CustomerFullName,
                    OrderersPhoneNumber = order.CustomerPhone,
                    OrderProperties = (BO.OrderProperties)order.OrderProperties,
                    OrderOpenDate = order.OrderDate,
                    deliveryPerOrderList = null,
                    MaximumDeliveryDate = maxDeliveryDate,
                    OrderStatus = BO.OrderStatus.Open,

                    ScheduleStatus =
                        maxDeliveryDate < config.Clock ? ScheduleStatus.Late
                        : maxDeliveryDate - config.DelayRiskTime < config.Clock
                            ? ScheduleStatus.InRisk
                            : ScheduleStatus.OnTime,

                    TimeLeftToCompleteOrder =
                        maxDeliveryDate >= config.Clock
                            ? maxDeliveryDate - config.Clock
                            : TimeSpan.Zero,

                    EstimatedDeliveryDate = null
                };
            }
            else
            {
                // Set BO.Order fields according to DO.Delivery
                bo = new()
                {
                    ID = order.Id,
                    OrderType = (BO.OrderType)order.OrderType,
                    VerbalDescription = order.OrderNote,
                    FullAddressOfTheOrder = order.CustomerAddress,
                    Latitude = order.Latitude,
                    Longitude = order.Longitude,

                    AirDistance = CalculateAirDistance(currentDelivery),

                    FullNameOfTheInviter = order.CustomerFullName,
                    OrderersPhoneNumber = order.CustomerPhone,
                    OrderProperties = (BO.OrderProperties)order.OrderProperties,
                    OrderOpenDate = order.OrderDate,

                    deliveryPerOrderList =
                        DeliveryManager
                            .GetList_DeliveryPerOrderInList(order.Id)!
                            .ToList(),

                    EstimatedDeliveryDate = Calc_EstimatedDeliveryDate(currentDelivery),

                    MaximumDeliveryDate = maxDeliveryDate,

                    OrderStatus = Calc_OrderStatus(currentDelivery),

                    ScheduleStatus = Calc_ScheduleStatus(maxDeliveryDate),

                    TimeLeftToCompleteOrder =
                        maxDeliveryDate >= config.Clock
                            ? maxDeliveryDate - config.Clock
                            : TimeSpan.Zero,
                };
            }

            return bo;
        }

        internal static BO.OpenOrderInList ConvertToOpenOrderInList(DO.Order order)
        {
            var config = AdminManager.GetConfig(); // צילום Config

            var currentDelivery = DeliveryManager.GetLastDelivery(order.Id);

            if (currentDelivery is null)
                throw new BO.BlDoesNotExistException(
                    $"Order {order.Id} has no active delivery");

            var maxDeliveryDate = order.OrderDate + config.MaxDeliveryDuration;

            return new BO.OpenOrderInList
            {
                courierId = currentDelivery.CourierId, // תיקון קטן (לא OrderId)
                OrderId = order.Id,

                OrderType = (BO.OrderType)order.OrderType,

                OrderProperties = (BO.OrderProperties)order.OrderProperties,

                CustomerAddress = order.CustomerAddress,

                AirDistance = CalculateAirDistance(currentDelivery),

                actualDistance = CalculateActualDistance(currentDelivery),

                EstimatedDeliveryTime =
                    config.Clock - (Calc_EstimatedDeliveryDate(currentDelivery) ?? config.Clock),

                ScheduleStatus = Calc_ScheduleStatus(maxDeliveryDate),

                deliveryTimeLeft =
                    maxDeliveryDate >= config.Clock
                        ? maxDeliveryDate - config.Clock
                        : TimeSpan.Zero,

                MaximumDeliveryTime = maxDeliveryDate
            };
        }

        #endregion

        internal static BO.OrderInProgress ConvertToOrderInProgress(DO.Delivery delivery, BO.Order order)
        {
            if (delivery == null)
                throw new BO.BlArgumentNullException("Delivery is null");

            var courier = CourierManager.Read(delivery.CourierId)
                ?? throw new BlDoesNotExistException(
                    $"courier with id:{delivery.CourierId} does not exist");

            return new BO.OrderInProgress
            {
                DeliveryId = delivery.Id,
                orderId = order.ID,

                orderType = order.OrderType,

                description = order.VerbalDescription,
                CustomerAddress = order.FullAddressOfTheOrder,

                actualDistance = delivery.ActualDistance ?? 0,
                AirDistance = order.AirDistance,

                OrderCreation = order.OrderOpenDate,
                DeliveryStart = delivery.DeliveryStartTime,

                ExpectedDeliveryTime =
                    order.EstimatedDeliveryDate ?? DateTime.MinValue,

                MaximumDeliveryTime = order.MaximumDeliveryDate,

                orderStatus = order.OrderStatus,

                ScheduleStatus = BO.ScheduleStatus.OnTime,

                deliveryTimeLeft = order.TimeLeftToCompleteOrder,

                CourierFullName = courier.FullName,
                CourierPhone = courier.Phone
            };
        }






        #region Calculation

        internal static double CalculateAirDistance(DO.Delivery delivery)
        {
            if (delivery == null)
                throw new BO.BlArgumentNullException("Delivery is null");

            var config = AdminManager.GetConfig();

            if (config == null || config.Latitude == null || config.Longitude == null)
                return 0;

            DO.Order doOrder;

            lock (AdminManager.BlMutex) // stage 7
                doOrder = s_dal.Order.Read(delivery.OrderId)
                    ?? throw new BO.BlDoesNotExistException(
                        $"Order {delivery.OrderId} does not exist");

            return Tools.CalculateAirDistance(
                config.Latitude.Value,
                config.Longitude.Value,
                doOrder.Latitude,
                doOrder.Longitude
            );
        }



        internal static double CalculateAirDistance(double latitude, double longitude)
        {
            var config = AdminManager.GetConfig();
            if (config == null || config.Latitude == null || config.Longitude == null)
                return 0;

            return Tools.CalculateAirDistance(
                config.Latitude.Value,
                config.Longitude.Value,
                latitude,
                longitude);
        }


        internal static double CalculateActualDistance(DO.Delivery delivery)
        {
            if (delivery == null)
                throw new BO.BlArgumentNullException("Delivery is null");

            // מביאים Config (נעול פנימית)
            var config = AdminManager.GetConfig();

            if (config == null ||
                config.Latitude == null ||
                config.Longitude == null)
                return 0;

            DO.Order doOrder;

            // קריאה ל-DAL חייבת lock
            lock (AdminManager.BlMutex) // stage 7
                doOrder = s_dal.Order.Read(delivery.OrderId)
                    ?? throw new BO.BlDoesNotExistException(
                        $"Order {delivery.OrderId} does not exist");

            // חישוב מרחק אווירי
            double airDistance = Tools.CalculateAirDistance(
                config.Latitude.Value,
                config.Longitude.Value,
                doOrder.Latitude,
                doOrder.Longitude);

            // פקטור כביש (בד"כ גדול מ-1)
            const double roadFactor = 1.2;

            return airDistance * roadFactor;
        }
        internal static DateTime? Calc_EstimatedDeliveryDate(DO.Delivery delivery)
        {
            if (delivery == null)
                throw new BO.BlArgumentNullException("Delivery is null");

            var config = AdminManager.GetConfig(); // צילום Config

            BO.DeliveryType type = (BO.DeliveryType)delivery.DeliveryType;

            double distance =
                delivery.ActualDistance ?? CalculateActualDistance(delivery);

            return type switch
            {
                BO.DeliveryType.None =>
                    delivery.DeliveryStartTime.AddHours(distance / config.AvgWalkingSpeed),

                BO.DeliveryType.Bicycle =>
                    delivery.DeliveryStartTime.AddHours(distance / config.AvgBicycleSpeed),

                BO.DeliveryType.Motorcycle =>
                    delivery.DeliveryStartTime.AddHours(distance / config.AvgMotorcycleSpeed),

                BO.DeliveryType.Car =>
                    delivery.DeliveryStartTime.AddHours(distance / config.AvgCarSpeed),

                _ => throw new BlInvalidStatusException("Unknown delivery type")
            };
        }


        internal static BO.OrderStatus Calc_OrderStatus(DO.Delivery delivery)
        {
            return (BO.DeliveryTerminationType)delivery.DeliveryTermintionType switch
            {
                BO.DeliveryTerminationType.Cancelled => OrderStatus.Cancelled,
                BO.DeliveryTerminationType.RefusedToAccept => OrderStatus.Refused,
                BO.DeliveryTerminationType.DeliveredSeccessfully => OrderStatus.Delivered,
                BO.DeliveryTerminationType.None => OrderStatus.Open,
                BO.DeliveryTerminationType.FailedToDeliver => OrderStatus.Open,
                BO.DeliveryTerminationType.CustomerNotHome => OrderStatus.Open,
                _ => throw new BlInvalidStatusException("Unknown delivery termination type")
            };
            
        }
        internal static BO.ScheduleStatus Calc_ScheduleStatus(DateTime maxDeliveryDate)
        {
            var config = AdminManager.GetConfig(); // צילום Config

            return maxDeliveryDate < config.Clock
                ? ScheduleStatus.Late
                : maxDeliveryDate - config.DelayRiskTime > config.Clock
                    ? ScheduleStatus.InRisk
                    : ScheduleStatus.OnTime;
        }


        /// This method is not permitted to delete orders and always throws a logical exception according to system requirements.



        #endregion
        internal static void HandleOrderInternal(int courierId, int orderId)
        {
            // בדיקה שההזמנה קיימת (כבר ננעלת בפנים)
            BO.Order order = GetOrderDetails(orderId);

            lock (AdminManager.BlMutex) // stage 7
            {
                // בדיקה שאין משלוח פעיל
                if (DeliveryManager.GetLastDelivery(orderId) != null)
                    throw new BlInvalidStatusException("Order is already being handled.");

                DO.Delivery newDelivery = new DO.Delivery
                {
                    Id = 0,
                    OrderId = orderId,
                    CourierId = courierId,
                    DeliveryStartTime = DateTime.Now,
                    DeliveryEndTime = null,
                    DeliveryTermintionType = DO.DeliveryTermintionType.None,
                    ActualDistance = null
                };

                s_dal.Delivery.Create(newDelivery);
            }
        }

    }
}
