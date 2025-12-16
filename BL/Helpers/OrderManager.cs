using DalApi;
using BO;

namespace Helpers
{
    internal static class OrderManager
    {
        private static IDal s_dal = Factory.Get;

        // Creates a new order in the data layer based on a business order object
        internal static void AddOrder(int applicantId, BO.Order boOrder)
        {
            if (boOrder == null)
                throw new ArgumentNullException(nameof(boOrder));

            DO.Order doOrder = new DO.Order(
                Id: 0,
                OrderType: (DO.OrderType)boOrder.features,
                OrderNote: boOrder.VerbalDescription ?? "",
                CustomerAddress: boOrder.FullAddressOfTheOrder ?? "",
                Latitude: boOrder.Latitude,
                Longitude: boOrder.Longitude,
                CustomerFullName: boOrder.FullNameOfTheInviter ?? "",
                CustomerPhone: boOrder.OrderersPhoneNumber ?? "",
                OrderDate: boOrder.OrderOpeningTime
            );

            s_dal.Order.Create(doOrder);
        }

        // Deletes an existing order from the data layer
        internal static void DeleteOrder(int applicantId, int orderId)
        {
            s_dal.Order.Delete(orderId);
        }

        // Updates editable details of an existing order
        internal static void UpdateOrder(int applicantId, BO.Order boOrder)
        {
            DO.Order oldOrder = s_dal.Order.Read(boOrder.ID);

            DO.Order updatedOrder = oldOrder with
            {
                OrderType = (DO.OrderType)boOrder.features,
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
                features = (BO.OrderType)doOrder.OrderType,
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
                    DeliveryType = DeliveryType.Foot,
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
        internal static BO.Order ConvertToOrder(DO.Order order)
        {
            BO.Order newOrder = new BO.Order
            {
                ID = order.Id,
                DeliveryType = (BO.DeliveryType)order.,
                OrderType = (BO.OrderType)order.OrderType,
                VerbalDescription = order.OrderNote,
                FullAddressOfTheOrder = order.CustomerAddress,
                Latitude = order.Latitude,
                Longitude = order.Longitude,
                FullNameOfTheInviter = order.CustomerFullName,
                OrderersPhoneNumber = order.CustomerPhone,
                OrderOpeningTime = order.OrderDate
            };

            return newOrder ?? throw new BO.BlDoesNotExistException($"Order with ID {order.ID} does not exist.");
        }
 
    }
}
