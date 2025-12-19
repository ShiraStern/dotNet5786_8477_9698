

namespace BO;
/// <summary>
/// Represents the details of a delivery associated with an order in a list.
/// </summary>
/// <remarks>This class provides information about a specific delivery, including its unique identifier,  the
/// courier responsible for the delivery, the type of delivery, and its start and end times.  It also includes the
/// termination status of the delivery.</remarks>
public class DeliveryPerOrderInList
{
    public int DeliveryId { get; init; }
    public int courierId { get; init; }
    public string? CourierName { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public DateTime DeliveryStart { get; init; }
    public DeliveryTerminationType DeliveryTerminationType { get; set; }
    public DateTime? DeliveryEndTime { get; set; }
}
