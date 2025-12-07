
namespace BO
{
    public class CourierInList
    {
        int ID { get; init; }
        string fullName { get; init; }

        bool active { get; set; }
        DeliveryType kinddelivery { get; set; }
        DateTime timeStartWork { get; set; }
        int numOfDeliveriesInTime { get; set; }
        int numOfDeliveriesNotInTime { get; set; }
        int? numOfDeliveriesInProcessing { get; set; }
        }
}

