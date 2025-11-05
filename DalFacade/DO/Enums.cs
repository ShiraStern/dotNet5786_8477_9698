namespace DO;

//// <summary>
/// Enum representing different types of delivery methods.
/// </summary>
public enum DeliveryType
{
    None,
    Drone,
    Motorcycle,
    Car
}

// <summary>
/// Enum representing different types of delivery types.
/// </summary>
public enum OrderType
{
    TypeA,
    TypeB,
    TypeC
}

public enum DeliveryTermintionType
{
    DeliveredSeccessfully,
    RefusedToAccept,
    Cancelled,
    CustomerNotHome,
    FailedToDeliver
}