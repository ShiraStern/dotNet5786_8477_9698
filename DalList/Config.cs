

namespace Dal;
/// <summary>
/// Provides configuration settings and utility methods for managing application-wide constants,  default values, and
/// stateful properties related to order and delivery processing.
/// </summary>
/// <remarks>This class includes both static constants and mutable properties for managing application 
/// configuration. It provides unique identifiers for orders and deliveries, as well as settings  for time, location,
/// and operational parameters such as speed and delivery thresholds.  Use the <see cref="Reset"/> method to restore
/// default values for mutable properties.</remarks>


/// //C:\Users\User\source\repos\dotNet5786_8477_9698\DalList\Config.cs
internal static class Config
{

    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    internal static int NextOrderId { get => nextOrderId++; }
    internal const int startDeliveryId = 100;
    private static int nextDeliveryId = startDeliveryId;
    internal static int NextDeliveryId { get => nextDeliveryId++; set => nextDeliveryId = value; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static int ManagerID { get; set; } = 216318477;
    internal static string ManagerPassword { get; set; } = "1111";
    internal static string? CompanyAddress { get; set; } = "בית הדפוס 9 ירושלים";
    internal static double? Latitude { get; set; } = null;
    internal static double? Longitude { get; set; } = null;
    internal static double? MaxRange { get; set; } =297; // in KM, in Israel
    internal static double AvgCarSpeed { get; set; } = 50;// in KM/H
    internal static double AvgMotorcycleSpeed { get; set; } = 60;// in KM/H
    internal static double AvgBicycleSpeed { get; set; } = 20;// in KM/H
    internal static double AvgWalkingSpeed { get; set; } = 6;// in KM/H
    internal static TimeSpan MaxDeliveryDuration { get; set; } = TimeSpan.FromDays(30);
    internal static TimeSpan DelayRiskTime { get; set; } = TimeSpan.FromDays(25);
    internal static TimeSpan InactivityThreshold { get; set; } = TimeSpan.FromDays(30);
    public const int MIN_ID = 200000000;
    public const int MAX_ID = 400000000;

    internal static void Reset()
    {
        nextOrderId= startOrderId;
        nextDeliveryId = startDeliveryId;
        Clock = DateTime.Now;
        CompanyAddress = null;
        Latitude = null;
        Longitude = null;
        MaxRange = null;
        AvgCarSpeed = 50;
        AvgBicycleSpeed = 60;
        AvgMotorcycleSpeed = 20;
        AvgWalkingSpeed = 6;
        MaxDeliveryDuration = TimeSpan.FromDays(30);
        DelayRiskTime = TimeSpan.FromDays(25);
        InactivityThreshold = TimeSpan.FromDays(30);
    }
}