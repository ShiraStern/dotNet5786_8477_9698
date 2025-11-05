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

/// <summary>
/// Specifies the possible outcomes for the termination of a delivery process.
/// </summary>
public enum DeliveryTermintionType
{
    DeliveredSeccessfully,
    RefusedToAccept,
    Cancelled,
    CustomerNotHome,
    FailedToDeliver
}