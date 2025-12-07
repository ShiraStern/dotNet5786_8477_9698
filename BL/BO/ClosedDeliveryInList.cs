
namespace BO;

public class ClosedDeliveryInList
{
    public int DeliveryId { get; init;}
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public string FullAddressOfDelivery { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public double ActualDistance { get; init;  }
    public TimeSpan TotalHandlingTime { get; init; }    
    public DeliveryTermintionType DeliveryTermintionType { get; init; } 

}
