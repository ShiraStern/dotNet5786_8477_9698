namespace BO;

public class OpenOrderInList
{
    public int courierId { get; init; }
    public int OrderId { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public OrderType OrderType { get; init; }
    public string? CustomerAddress { get; init; }
    public double AirDistance { get; init; }
    public double actualDistance { get; init; }
    public TimeSpan? EstimatedDeliveryTime { get; set; }
    public ScheduleStatus ScheduleStatus { get; set; }
    public TimeSpan? deliveryTimeLeft { get; set; }    
    public DateTime MaximumDeliveryTime { get; init; } 

}
