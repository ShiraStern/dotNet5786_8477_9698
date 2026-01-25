
namespace BO
{
    public class CourierInList
    {
        public int ID { get; init; }
        public string? FullName { get; init; }
        public bool Active { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public int NumOfDeliveriesOnTime { get; set; }
        public int NumOfDeliveriesNotOnTime { get; set; }
        public int? IdOfDeliveryInProcess { get; set; }

        }
}

