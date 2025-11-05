
namespace Dal;
/// <summary>
/// Provides configuration settings and utility methods for managing application-wide constants, identifiers, and state.
/// This class includes functionality for generating unique IDs, managing the application clock, and storing global
/// settings such as manager credentials and company information.
/// </summary>

 internal static class Config
{
    /// <summary>
    /// Represents the starting order ID used as the initial value for order processing.
    /// </summary>
    /// <remarks>This constant is used as the base value for generating order IDs. It is intended for internal
    /// use only.</remarks>
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    internal static int NextOrderId { get => nextOrderId++; }

    /// <summary>
    /// Represents the starting identifier for delivery ID.
    /// </summary>
    /// <remarks>This constant is used as the initial value for delivery-related identifiers. It is intended
    /// for internal use and should not be modified.</remarks>
    internal const int startDeliveryId = 100;
    private static int nextDeliveryId = startDeliveryId;
    internal static int NextDeliveryId { get => nextDeliveryId++; }
    
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static int ManagerID = int.Parse(Console.ReadLine()??"0") ;
    internal static string ManagerPassword = Console.ReadLine()??"";/// תוספת
    internal static string? CompanyAddress = Console.ReadLine();
    internal static double? Latitude=null;
    internal static double? Longitude = null;
    internal static double? MaxRange= null;
    internal static double AvgCarSpeed;
    internal static double AvgMotorcycleSpeed;
    internal static double AvgDroneSpeed;
    internal static double AvgWalkingSpeed;
    internal static DateTime MaxDeliveryDuration;
    internal static DateTime DelayRiskTime;
    internal static DateTime InactivityThreshold;
/// <summary>
/// Resets the system state to its initial configuration.
    internal static void Reset()
    {
        nextOrderId = startOrderId;
        nextDeliveryId = startDeliveryId;
        CompanyAddress= null;
        Latitude= null;
        Longitude=  null;
        MaxRange= null;
        AvgCarSpeed= null;
        AvgDroneSpeed = null;
        AvgMotorcycleSpeed= null;
        AvgWalkingSpeed= null;
        MaxDeliveryDuration= null;
        DelayRiskTime= null;
        InactivityThreshold= null;
    }


}
