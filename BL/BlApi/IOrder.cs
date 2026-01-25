using BO;
using System.Threading.Tasks;

namespace BlApi;
public interface IOrder : IObservable //stage 5 ממשק הרחבת
{
    IEnumerable<int> GetOrdersStatusCounts(int applicantId);
    IEnumerable<BO.OrderInList> GetOrderList(int applicantId,
        BO.filterOrdersByProperty? filterOrdersBy=null, object? type=null, BO.sortOrdersByProperty? sortOrdersBy = null);
    BO.Order GetDetails(int applicantId, int orderId);
    void UpdateDetails(int applicantId, BO.Order boOrder);
    void CancelOrder(int applicantId, int orderId);
    void Delete(int applicantId, int id);
    Task AddOrderAsync(int applicantId, BO.Order boOrder);
    void EndOrderHandle(int applicantId,int courierId, int orderId, int deliveryId);
    void HandleOrder(int applicantId, int courierId, int orderId);
    IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null);
    IEnumerable<BO.OpenOrderInList> GetDeliveriesPerCourier(int applicantId, int courierId, BO.OrderType? filterOrderByType = null, sortClosedDeliveriesByProperty? byProperty = null);
    public BO.OrderInProgress GetOrderInProgress(int deliveryID, BO.Order order);
}
