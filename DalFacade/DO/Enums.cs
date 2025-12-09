namespace DO;

//// <summary>
/// Enum representing different types of delivery methods.
/// </summary>
public enum DeliveryType
{
    None,
    Motorcycle,
    Car
}

// <summary>
/// Enum representing different types of delivery types.
/// </summary>
public enum OrderType
{
    Large,
    Medium,
    Small
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
public enum MainMenu
{
    Exit,
    CourierMenu,
    OrderMenu,
    DeliveryMenu,
    ConfigMenu,
    InitializationData,
    ViewData,
    ResetData,
}

public enum CourierMenu
{
    Exit=1,
    AddCourier,
    ViewCourier,
    ViewAllCouriers,
    UpdateCourier,
    DeleteCourier,
    DeleteAllCouriers,
}
public enum OrderMenu
{
    Exit = 1,
    AddOrder,
    ViewOrders,
    ViewAllOrders,
    UpdateOrder,
    DeleteOrder,
    DeleteAllOrders,
}
public enum DeliveryMenu
{
    Exit=1,
    AddDelivery,
    ViewDelivery,
    ViewAllDeliveries,
    UpdateDelivery,
    DeleteDelivery,
    DeleteAllDeliveries,
}
public enum ConfigMenu
{
    Exit=1,
    AddMiniuteToClock,
    AddHourToClock,
    AddDayToClock,
    AddMonthToClock,
    AddYearToClock,
    ViewClock,
    UpDateMaxRange,
    ViewMaxDeliveryDuration,
    ResetConfig
}