
namespace BO
{
    public class Courier
    {
        public int ID { get; init; }
        public string  FullName { get; init; }
        public string  PhoneNember { get; init; }
        public string  Email { get; init; }
        public string  Password { get; init; }
        public bool Active { get; set; }
        public double? MaxDistance { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public int NumOfDeliveriesInTime { get; set; }
        public int NumOfDeliveriesNotInTime { get; set; }
        public BO.OrderInProgress? OrderInProgress { get; set; }

        
    }


}
