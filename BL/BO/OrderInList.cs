
using DO;

namespace BO;
/// <summary>
/// Represents an order within a delivery list, including its associated details such as delivery type, status, and
/// timing information.
/// </summary>
/// <remarks>This class provides a structured representation of an order in the context of a delivery operation. 
/// It includes properties for identifying the order, tracking its status, and calculating delivery-related
/// metrics.</remarks>

public class OrderInList
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public double AirDistance { get; init; }
    public OrderStatus OrderStatus { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan DeliveryTimeLeft { get; set; }
    public TimeSpan? TotalHandlingTime { get; set; }
    public int TotalDeliveries { get; set; }
}


