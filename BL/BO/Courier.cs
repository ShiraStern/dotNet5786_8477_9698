
namespace BO
{
    public class Courier
    {
        int ID { get; init; }
        string  fullName { get; init; }
        string  fhoneNember { get; init; }
        string  email { get; init; }
        string  password { get; init; }
        bool active { get; set; }
        double? maxdistance { get; set; }
        DeliveryType kinddelivery { get; set; }
        DateTime timeStartWork { get; set; }
        int numOfDeliveriesInTime { get; set; }
        int numOfDeliveriesNotInTime { get; set; }
        BO.OrderInProgress? orderInProgress { get; set; }
    }


}
