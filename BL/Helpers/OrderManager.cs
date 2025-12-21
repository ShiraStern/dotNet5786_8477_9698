using BlApi;
using BO;
using DO;
using System.Xml;

namespace Helpers
{
    internal static class OrderManager
    {
        private static DalApi.IDal s_dal = DalApi.Factory.Get;

        // Creates a new order in the data layer based on a business order object
        // Plan / Pseudocode:
        // 1. Validate input (throw BlArgumentNullException if boOrder is null).
        // 2. Map BO.Order fields to DO.Order, providing safe defaults for nullable strings.
        // 3. Try to create the DO.Order via s_dal.Order.Create.
        // 4. Catch specific DAL exceptions and rethrow BL-layer exceptions with clear, informative messages
        //    including context (customer name, address, order id when available) and the original exception as inner.
        // 5. Preserve original exception as innerException for debugging and logging.
        internal static void AddOrder(int applicantId, BO.Order boOrder)
        {
            if (boOrder == null)
                throw new BlArgumentNullException(nameof(boOrder));

            DO.Order doOrder = new DO.Order(
                Id: 0,
                OrderType: (DO.OrderType)boOrder.OrderType,
                OrderNote: boOrder.VerbalDescription ?? string.Empty,
                CustomerAddress: boOrder.FullAddressOfTheOrder ?? "",
                Latitude: boOrder.Latitude,
                Longitude: boOrder.Longitude,
                CustomerFullName: boOrder.FullNameOfTheInviter ?? "",
                CustomerPhone: boOrder.OrderersPhoneNumber ?? "",
                OrderDate: boOrder.OrderOpeningTime,
                OrderProperties: DO.OrderProperties.None
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
        }

        // Deletes an existing order from the data layer
        internal static void DeleteOrder(int applicantId, int orderId)
        {
            try
            {
            s_dal.Order.Delete(orderId);
        }
            catch (DalXMLFileLoadCreateException ex)
            {
                // Provide clear description that creation failed due to XML/file issues in DAL
                throw new BlDataAccessException("Failed to delete order: data layer XML/file load or create error.", ex);
            }
            catch (DalDoesNotExistException ex)
            {
                // Provide context about the conflicting order to help debugging
                string context = $"Order does not exist '{orderId}'.";
                throw new BlDoesNotExistException(context, ex);
            }
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
                OrderOpeningTime = doOrder.OrderDate
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
                    DeliveryType = BO.DeliveryType.Foot,
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
        internal static DO.Order ConvertToOrder(BO.Order order)
        {
            DO.Order newOrder = s_dal.Order.Read(order.ID);
            return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.ID} does not exist.");
        }
        internal static BO.Order ConvertToOrder(DO.Order order, DO.Delivery? delivery)// מטודת עזר גם ל cancelOrder
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
                OrderOpeningTime = order.OrderDate,
                deliveryPerOrderList = new List<DeliveryPerOrderInList>()
            };

            // אין משלוח → הזמנה פתוחה
            if (delivery == null)
            {
                boOrder.OrderStatus = OrderStatus.Open;
                boOrder.ScheduleStatus = ScheduleStatus.OnTime;
                boOrder.TimeLeftToCompleteOrder = AdminManager.MaxDeliveryDuration;
                return boOrder;
            }

            // יש משלוח
            boOrder.AirDistance = delivery.ActualDistance ?? 0;

            // משלוח פעיל
            if (delivery.DeliveryEndTime == null)
            {
                boOrder.OrderStatus = OrderStatus.InTreatment;
                boOrder.ScheduleStatus = ScheduleStatus.OnTime;
                boOrder.MaximumDeliveryTime =
                    delivery.DeliveryStartTime.Add(AdminManager.MaxDeliveryDuration);

                boOrder.TimeLeftToCompleteOrder =
                    boOrder.MaximumDeliveryTime - DateTime.Now;

                return boOrder;
            }

            // משלוח הסתיים
            boOrder.TimeLeftToCompleteOrder = TimeSpan.Zero;

            if (delivery.DeliveryTermintionType == DO.DeliveryTermintionType.Cancelled)
            {
                boOrder.OrderStatus = OrderStatus.Cancelled;
            }
            else
            {
                boOrder.OrderStatus = OrderStatus.Delivered;
            }


            boOrder.ScheduleStatus = ScheduleStatus.OnTime;

            return boOrder;
        }
        internal static void CancelOrder(int orderId)//מטודת עזר ל cancelOrder
        {
            // 1. קריאת ההזמנה
            DO.Order doOrder = s_dal.Order.Read(orderId);

            // 2. המרה ל-BO כדי לדעת סטטוס לוגי
            BO.Order boOrder = GetBoOrder(doOrder);

            // 3. בדיקת חוקיות
            if (boOrder.OrderStatus != OrderStatus.Open &&
                boOrder.OrderStatus != OrderStatus.InTreatment)
            {
                throw new BO.BlInvalidStatusException(
                    $"Order {orderId} cannot be cancelled in status {boOrder.OrderStatus}");
            }

            DateTime now = DateTime.Now;

            // 4. אם ההזמנה פתוחה – יצירת משלוח מדומה
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

            // 5. אם ההזמנה בטיפול – עדכון משלוח קיים
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
        }

        internal static BO.Order GetBoOrder(DO.Order order)
        {
            List<DO.Delivery>? deliveries =
                DeliveryManager.GetDoDeliveriesByOrderId(order.Id);

            //  אין משלוחים – הזמנה פתוחה
            if (deliveries is null || deliveries.Count == 0)
            {
                return new BO.Order
                {
                    ID = order.Id,
                    OrderStatus = OrderStatus.Open,
                    ScheduleStatus = ScheduleStatus.OnTime,
                    OrderOpeningTime = order.OrderDate,
                    // שדות נוספים לפי הצורך
                };
            }

            //  יש משלוחים – מחשבים לפי delivery האחרון
            DO.Delivery lastDelivery = deliveries.Last();
            BO.Order newOrder = ConvertToOrder(order, lastDelivery);
            return newOrder;
        }

        internal static IEnumerable<int> GetOrdersStatusCountsInternal(int applicantId)//פונקצית עזר לפונקציה GetOrdersStatusCounts 
        {
            int[] result = new int[9];

            IEnumerable<DO.Order> doOrders = s_dal.Order.ReadAll();

            IEnumerable<BO.Order> ordersOfApplicant = doOrders
                .Select(o => GetBoOrder(o))
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
                    .Select(o => GetBoOrder(o));
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
                        boOrders.OrderBy(o => o.OrderOpeningTime),

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
            return GetBoOrder(doOrder);
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
                OrderDate = boOrder.OrderOpeningTime
            };

            s_dal.Order.Update(updatedOrder);
        }

    }
}
