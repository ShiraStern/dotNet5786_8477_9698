
namespace DalTest;
using DalApi;
using DO;  
public static class Initialization
{
    private static ICourier? s_dalCourier; //stage 1
    private static IDelivery? s_dalDelivery; //stage 1
    private static IOrder? s_dalOrder; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_random = new();
    private static void createCouriers()
    { }

    private static void createDelivery() { }
    private static void createOrders() { }

}
