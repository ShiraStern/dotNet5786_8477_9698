using BO;

namespace BlApi;
public interface IOrder
{
    IEnumerable<int> GetOrdersStatusCounts(int applicantId);
    IEnumerable<BO.OrderInList> GetOrderList(int applicantId, BO.filterOrdersByProperty? filterOrdersBy=null, object? type=null, BO.sortOrdersByProperty? sortOrdersBy = null);
    BO.Order GetDetails(int applicantId, int orderId);
    void UpdateDetails(int applicantId, BO.Order boOrder);
    void cancelOrder(int applicantId, int orderId);
    void Delete(int applicantId, int id);
    void AddOrder(int applicantId, BO.Order boOrder);
    void EndOrderHandle(int applicantId,int courierId, int orderId, int deliveryId);
    void HandleOrder(int applicantId, int courierId, int orderId);
    IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null);
    IEnumerable<BO.OpenOrderInList> GetDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null);


}
