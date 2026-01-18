namespace DO;

//// <summary>
/// Enum representing different types of delivery methods.
/// </summary>
public enum DeliveryType
{
    None, // walking
    Bicycle,
    Motorcycle,
    Car
}

// <summary>
/// Enum representing different types of order types.
/// </summary>
public enum OrderType
{
    Large,
    Medium,
    Small
}
/// <summary>
/// 
/// </summary>
public enum OrderProperties
{
    None,
    Fragile, //שביר
    Weighty,// כבד משקל
    FragileAndWeighty //כבד ושביר
    
}

/// <summary>
/// Specifies the possible outcomes for the termination of a delivery process.
/// </summary>
public enum DeliveryTermintionType
{
    None,
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