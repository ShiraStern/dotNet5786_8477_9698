
namespace BO;

/// <summary>
/// Enum representing vehicle types (duplicated from DO for separation of concerns).
/// </summary>
public enum DeliveryType
{
    Motorcycle,
    Car,
    Bicycle,
    Van,
    Foot
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
    Minute,
    Hour,
    Day,
    Month,
    Year
}

public enum sortCouriersByProperty
{
    IsActive,
    NumberOfDeliveries,
    //* Add more properties as needed
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
    OrderStatus,
    DeliveryType,
    OrderType
    //* Add more properties as needed
}
 public enum sortClosedDeliveriesByProperty
{

}

//old version
//
//namespace BO;
//public enum DeliveryType
//{
//    None,
//    Drone,
//    Motorcycle,
//    Car
//}

//public enum OrderType
//{
//    Regular,
//    Fast,
//    Emergency
//}
//public enum OrderStatus
//{
//    DeliveredSeccessfully,
//    RefusedToAccept,
//    Cancelled,
//    CustomerNotHome,
//    FailedToDeliver
//}

//public enum DeliveryTermintionType
//{
//    DeliveredSeccessfully,
//    RefusedToAccept,
//    Cancelled,
//    CustomerNotHome,
//    FailedToDeliver
//}
//public enum ScheduleStatus
//{

//}

//public enum OrderProperties
//{
//    Large,
//    Medium,
//    Small
//}
