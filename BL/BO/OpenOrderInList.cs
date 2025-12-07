namespace BO;

public class OpenOrderInList
{
    int courierId { get; init; }    
    int OrderId { get; init; }
    DeliveryType DeliveryType { get; init; }
    OrderType OrderType { get; init; }
    string CustomerAddress { get; init; }
    double AirDistance { get; init; }   
    double actualDistance { get; init; }
    TimeSpan? EstimatedDeliveryTime { get; set; }
    ScheduleStatus ScheduleStatus { get; set; }
    TimeSpan? deliveryTimeLeft { get; set; }    
    DateTime MaximumDeliveryTime { get; init; } 

}
