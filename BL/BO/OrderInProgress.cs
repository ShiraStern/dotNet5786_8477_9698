

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
    public int Id { init; get; }   
    public string? CustomerFullName { init; get; }
    public string? Phone { init; get; } 
    public string? email { init; get; }
    public string? password { init; get; }  
    public bool IsActive { init; get; }
    public double? MaxDeliveryDistance { init; get; }
    public DeliveryType deliveryType { init; get; }  
    public DateTime EmploymentStartDate { init; get; }
    public int NumberOfOrdersInProgress { init; get; }
    int DeliveryId { get; init; }
    int orderId { get; init; }
    OrderType orderType { get; init; }
    string? description { get; set; }
    string? CustomerAddress{ get; init; }
    double AirDistance { get; init; }
    double actualDistance { get; init; }
    string? CourierFullName { get; init; }
    string? CourierPhone { get; init; }
    DateTime OrderCreation { get; init; }
    DateTime DeliveryStart { get; init; }
    DateTime ExpectedDeliveryTime { get; init; }
    DateTime MaximumDeliveryTime { get; init; }
    OrderStatus orderStatus { get; set; }
    ScheduleStatus ScheduleStatus { get; set; }
    TimeSpan? deliveryTimeLeft { get; set; }

}


