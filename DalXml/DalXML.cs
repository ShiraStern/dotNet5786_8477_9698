
namespace Dal;
using Dal;
using DalApi;
using System.Diagnostics;


sealed internal class DalXML : IDal
{
    public static IDal Instance { get; } = new DalXML();
    private DalXML() { }    
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
