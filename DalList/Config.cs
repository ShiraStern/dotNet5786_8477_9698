
namespace Dal;

 internal static class Config
{
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    internal static int NextOrderId { get => nextOrderId++; }

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
    internal static double MaxRange= null;
    internal static double? MaxRange= null;
    internal static double? MaxRange= null;
    internal static double? MaxRange= null;







    internal static void Reset()
    {
        nextOrderId = startOrderId;
        //...
        Clock = DateTime.Now;
        //...
    }


}
