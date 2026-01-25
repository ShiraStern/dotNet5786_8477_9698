

using DO;

namespace BO;
/// <summary>
/// Represents an order that is currently in progress, including details about the delivery, courier, and scheduling
/// status.
/// </summary>
/// <remarks>This class provides information about an ongoing order, such as the delivery ID, customer address,
/// courier details,  and various timestamps related to the order's progress. It also includes the current status of the
/// order and its schedule.</remarks>
public class OrderInProgress
{
    public int DeliveryId { get; init; }
    public int orderId { get; init; }
    public OrderType orderType { get; init; }
    public string? description { get; set; }
    public string? CustomerAddress{ get; init; }
    public double AirDistance { get; init; }
    public double actualDistance { get; init; }
    public string? CourierFullName { get; init; }
    public string? CourierPhone { get; init; }
    public DateTime OrderCreation { get; init; }
    public DateTime DeliveryStart { get; init; }
    public DateTime ExpectedDeliveryTime { get; init; }
    public DateTime MaximumDeliveryTime { get; init; }
   public  OrderStatus orderStatus { get; set; }
   public  ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan? deliveryTimeLeft { get; set; }

}


