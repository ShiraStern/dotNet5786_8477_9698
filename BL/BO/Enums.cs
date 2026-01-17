
namespace BO;

/// <summary>
/// Enum representing vehicle types (duplicated from DO for separation of concerns).
/// </summary>
public enum DeliveryType
{
    None, // walking
    Bicycle,
    Motorcycle,
    Car
}

public enum OrderProperties
{
    None,
    Fragile, //שביר
    Weighty,// כבד משקל
    FragileAndWeighty //כבד ושביר
}


/// <summary>
/// Enum representing order types/sizes.
/// </summary>
public enum OrderType
{
    Small,
    Medium,
    Large
}

/// <summary>
/// Logical status of an order, derived from its delivery status.
/// </summary>
public enum OrderStatus
{
    Open,           // נוצרה אך טרם שויכה לשליח
    InTreatment,    // שויכה לשליח (בטיפול)
    Delivered,      // סופקה בהצלחה
    Refused,        // הלקוח סירב לקבל
    Cancelled       // בוטלה
}

/// <summary>
/// Status regarding the schedule and deadlines of the order.
/// </summary>
public enum ScheduleStatus
{
    OnTime, // בזמן
    InRisk, // בסיכון (מתקרב לזמן האספקה המירבי)
    Late    // באיחור
}

/// <summary>
/// Delivery termination result (Duplicated from DO).
/// </summary>
public enum DeliveryTerminationType
{
    DeliveredSuccessfully,
    RefusedToAccept,
    Cancelled,
    CustomerNotHome,
    FailedToDeliver
}

/// <summary>
/// Time units for clock manipulation in the management interface.
/// </summary>
public enum TimeUnit
{
    Minute=1,
    Hour,
    Day,
    Month,
    Year
}

public enum sortCouriersByProperty
{
    IsActive,
    IsNotActive
    
}
public enum FilterCouriersByProperty
{
    IsActive,
    IsNotActive,
    All
}

public enum sortOrdersByProperty
{
    OrderDate,
    DeliveryDate,
    CustomerName,
    OrderStatus
    //* Add more properties as needed
}

public enum filterOrdersByProperty
{
    All,
    OrderStatus,
    DeliveryType,
    OrderType
    //* Add more properties as needed
}
 public enum sortClosedDeliveriesByProperty
{
    OrderType,
    DeliveryEndTime,
    DeliveryTerminationType,
    ScheduleStatus
}
 public enum AdminMenuOptions
{
    Exit=1 ,
    GetClock,
    ForwardClock,
    ResetDataBase,
    InitializeDataBase,
    GetConfig,
    SetConfig
}

public enum CoureirMenuOptions
{
    Exit=1,
    GetDetails,
    UpdateDetails,
    GetCourierList,
    DeleteCourier,
    Login,
    AddCourier
}

public enum MainMenuOptions
{
    Exit,
    CourierMenu,
    OrderMenu,
    AdminMenu
}

public enum OrderMenuOptions
{
    Exit=1,
    GetOrdersStatusCounts,
    GetOrderList,
    GetDetails,
    UpdateDetails,
    CancelOrder,
    Delete,
    AddOrder,
    EndOrderHandle,
    HandleOrder,
    GetClosedDeliveriesPerCourier,
    GetDeliveriesPerCourier
}
