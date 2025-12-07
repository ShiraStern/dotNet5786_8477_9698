
namespace BO;
public enum DeliveryType
{
    None,
    Drone,
    Motorcycle,
    Car
}

public enum OrderType
{
    Regular,
    Fast,
    Emergency
}
public enum OrderStatus
{
    DeliveredSeccessfully,
    RefusedToAccept,
    Cancelled,
    CustomerNotHome,
    FailedToDeliver
}

public enum DeliveryTermintionType
{
    DeliveredSeccessfully,
    RefusedToAccept,
    Cancelled,
    CustomerNotHome,
    FailedToDeliver
}
public enum ScheduleStatus
{

}

public enum OrderProperties
{
    Large,
    Medium,
    Small
}
