
namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;

internal class OrderImplementation : IOrder
{
    public void AddOrder(int applicantId, Order boOrder)
    {
        throw new NotImplementedException();
    }

    public void cancelOrder(int applicantId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void Delete(int applicantId, int id)
    {
        throw new NotImplementedException();
    }

    public void EndOrderHandle(int applicantId, int courierId, int orderId, int deliveryId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesPerCourier(int applicantId, int courierId, OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenOrderInList> GetDeliveriesPerCourier(int applicantId, int courierId, OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null)
    {
        throw new NotImplementedException();
    }

    public Order GetDetails(int applicantId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OrderInList> GetOrderList(int applicantId, filterOrdersByProperty? filterOrdersBy = null, object? type = null, sortOrdersByProperty? sortOrdersBy = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<int> GetOrdersStatusCounts(int applicantId)
    {

    }

    public void HandleOrder(int applicantId, int courierId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void UpdateDetails(int applicantId, Order boOrder)
    {
        throw new NotImplementedException();
    }
}
