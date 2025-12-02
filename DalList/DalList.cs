
namespace Dal;
using DalApi;
using Dal;
 sealed internal class DalList : IDal

{
    public static IDal Instance { get; } = new DalList();
    public DalList() { }  
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
