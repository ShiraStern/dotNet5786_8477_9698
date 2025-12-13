
namespace Dal;
using Dal;
using DalApi;
using System.Diagnostics;


sealed internal class DalXml : IDal
{
    private static readonly Lazy<IDal>_instance
        =new Lazy<IDal>(() => new DalXml(),LazyThreadSafetyMode.ExecutionAndPublication);//זה הבונוס - להוסיף סינגלטאון עצל כלומר פה יש פונקציה שרק כאשר קוראים לה רק אז זה מקצה את האובייקט  
    public static IDal Instance => _instance.Value;// זה מוגדר כציבורי ומפה המשתמש יקרא לפונקציה כדי שתגדיר את האובייקט
    private DalXml() { }    
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
