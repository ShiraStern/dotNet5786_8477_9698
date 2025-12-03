
namespace Dal;
using DalApi;
using Dal;
 sealed internal class DalList : IDal

{
    private static readonly Lazy<IDal> _instance =
        new Lazy<IDal>(() => new DalList(), LazyThreadSafetyMode.ExecutionAndPublication); // זה הבונוס - להוסיף סינגלטאון עצל כלומר פה יש פונקציה שרק כאשר קוראים לה רק אז זה מקצה את האובייקט  

    public static IDal Instance => _instance.Value;
    private DalList() { }  
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
