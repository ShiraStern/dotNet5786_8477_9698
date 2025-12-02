
namespace Dal;
using DalApi;
using Dal;
 sealed internal class DalLists : IDal

{
    public static IDal Instance { get; } = new DalLists();
    private DalLists() { }  
    public IOrder Order { get; } = new OrederImplementation();

    public ICourier Courier { get; } = new CourierImplementation();

    public IDelivery Delivery { get; } = new DeliveryImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Order.DeleteAll();
        Delivery.DeleteAll();
        Courier.DeleteAll();
        Config.Reset();
    }
}
