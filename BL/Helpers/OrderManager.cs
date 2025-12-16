using BlApi;
using System.Xml;

namespace Helpers
{
    internal static class OrderManager
    {
        private static DalApi.IDal s_dal = Factory.Get;

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
                OrderProperties: OrderProperties.None
            );

            s_dal.Order.Create(doOrder);
        }
            catch (DalXMLFileLoadCreateException ex)
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

        // Updates editable details of an existing order
        internal static void UpdateOrder(int applicantId, BO.Order boOrder)
        {
            DO.Order oldOrder = s_dal.Order.Read(boOrder.ID);

            DO.Order updatedOrder = oldOrder with
            {
                OrderType = (DO.OrderType)boOrder.OrderType,
                OrderNote = boOrder.VerbalDescription ?? oldOrder.OrderNote,
                CustomerAddress = boOrder.FullAddressOfTheOrder ?? oldOrder.CustomerAddress,
                CustomerFullName = boOrder.FullNameOfTheInviter ?? oldOrder.CustomerFullName,
                CustomerPhone = boOrder.OrderersPhoneNumber ?? oldOrder.CustomerPhone
            };

            s_dal.Order.Update(updatedOrder);
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
        internal static BO.Order ConvertToOrder(DO.Order order, DO.Delivery? delivery) //לקחת את כל הנתונים מהדליברי והאורדר ולחשב אותם לפי המסמך הכללי זה בעצם הפונקציה שממירה את האורדר לדליבריי 
        {
            double size=1
                ;

            if (delivery == null)
            {
                throw new BO.BlArgumentNullException($"Delivery is null.");
            }
            else
            {
                switch ((BO.DeliveryType)delivery.DeliveryType)
                { 
                    case BO.DeliveryType.None: size=6; break;
                    case BO.DeliveryType.Bicycle: size = 20; break;
                    case BO.DeliveryType.Motorcycle: size = 60; break;
                    case BO.DeliveryType.Car: size = 50; break;
                }

                BO.Order newOrder = new BO.Order
                {
                    ID = order.Id,
                    DeliveryType = (BO.DeliveryType)delivery.DeliveryType,
                    VerbalDescription = order.OrderNote, 
                    FullAddressOfTheOrder = order.CustomerAddress,
                    Latitude = order.Latitude,
                    Longitude = order.Longitude,
                    AirDistance =(double) delivery.ActualDistance!,                    /*    public double AirDistance { get; set; } */
                    FullNameOfTheInviter = order.CustomerFullName,
                    OrderersPhoneNumber = order.CustomerPhone,
                    OrderType = (BO.OrderType)order.OrderType,
                    OrderOpeningTime = order.OrderDate,
                    EstimatedDeliveryTime= delivery.DeliveryStartTime.AddHours((double)delivery.ActualDistance/size),
                    MaximumDeliveryTime= delivery.DeliveryStartTime.Add(AdminManager.MaxDeliveryDuration),//                public DateTime MaximumDeliveryTime { get; set; }
                    OrderStatus=order.OrderStatus,//                public OrderStatus OrderStatus { get; set; }
                    ScheduleStatus=order.ScheduleStatus,//                public ScheduleStatus ScheduleStatus { get; set; }
                    TimeLeftToCompleteOrder=order.TimeLeftToCompleteOrder,//                public TimeSpan TimeLeftToCompleteOrder { get; set; }
                    deliveryPerOrderLis=order.deliveryPerOrderList,//              
                };

                return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.Id} does not exist.");

            }



        }
        internal static BO.Order GetBoOrder(DO.Order order)
        {
            List <DO.Delivery>? deliveries = DeliveryManager.GetDoDeliveriesByOrderId(order.Id)  ;
            if(deliveries is null)
                // הסטטוס של ההזמנה הזו יהיה פתוח 
            return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.Id} does not exist.");
        }
    }
}
