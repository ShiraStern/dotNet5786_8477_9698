namespace BlImplementation;

using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class OrderImplementation : IOrder
{
    // Adds a new order to the system

    public void AddOrder(int applicantId, Order boOrder)
    {
        OrderManager.AddOrder(applicantId, boOrder);
    }

    // Cancels an existing order (not implemented at this stage)
    public void cancelOrder(int applicantId, int orderId)
    {
        if(!AdminManager.IsValidManagerId(applicantId))
        {
            throw new BO.BlUnauthorizedAccessException("Only managers can cancel orders at this stage");
        }
        DO.Order? order = AdminManager.GetDal().Order.Read(orderId);
        DO.Delivery? delivery = DeliveryManager.GetDoDeliveryByOrderId(orderId);
        BO.Order order = OrderManager.


    }

    // Deletes an order from the system
    public void Delete(int applicantId, int id)
    {
        OrderManager.DeleteOrder(applicantId, id);
    }

    // Assigns an order to a courier (handled in delivery stage)
    public void HandleOrder(int applicantId, int courierId, int orderId)
    {
        throw new NotImplementedException("Handle order logic is implemented in Delivery stage");
    }

    // Completes order handling after delivery (handled in delivery stage)
    public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)
    {
        throw new NotImplementedException("End order handle logic is implemented in Delivery stage");
    }

    // Returns full details of a specific order
    public Order GetDetails(int applicantId, int orderId)
    {
        return OrderManager.GetOrderDetails(applicantId, orderId);
    }

    // Returns a list of orders with optional filtering and sorting
    public IEnumerable<OrderInList> GetOrderList(
        int applicantId,
        filterOrdersByProperty? filterOrdersBy = null,
        object? type = null,
        sortOrdersByProperty? sortOrdersBy = null)
    {
        return OrderManager.GetOrderList(applicantId, filterOrdersBy, type, sortOrdersBy);
    }

    // Returns counts of orders grouped by status
    public IEnumerable<int> GetOrdersStatusCounts(int applicantId)
    {
        return OrderManager.GetOrdersStatusCounts(applicantId);
    }

    // Returns open deliveries for a specific courier (handled in delivery stage)
    public IEnumerable<OpenOrderInList> GetDeliveriesPerCourier(
        int applicantId,
        int courierId,
        OrderType? filterOrderByType = null,
        sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException("Open deliveries per courier are handled in Delivery stage");
    }

    // Returns closed deliveries for a specific courier (handled in delivery stage)
    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesPerCourier(
        int applicantId,
        int courierId,
        OrderType? filterOrderByType = null,
        sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException("Closed deliveries per courier are handled in Delivery stage");
    }

    // Updates details of an existing order
    public void UpdateDetails(int applicantId, Order boOrder)
    {
        OrderManager.UpdateOrder(applicantId, boOrder);
    }
}
