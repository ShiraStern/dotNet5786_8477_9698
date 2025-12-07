

namespace BO;
/// <summary>
/// Represents the details of a delivery associated with an order in a list.
/// </summary>
/// <remarks>This class provides information about a specific delivery, including its unique identifier,  the
/// courier responsible for the delivery, the type of delivery, and its start and end times.  It also includes the
/// termination status of the delivery.</remarks>
public class DeliveryPerOrderInList
{
    int DeliveryId { get; init; }
    int courierId { get; init; }    
    string CourierName { get; init; }
    DeliveryType DeliveryType { get; init; }
    DateTime DeliveryStart { get; init; }
    DeliveryTermintionType deliveryTermintionType { get; set; } 
    DateTime? DeliveryEndTime { get; set; }
}
