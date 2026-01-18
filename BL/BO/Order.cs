
using DO;

namespace BO
{
    public class Order
    {
        public int ID { get; set; }        // הופך ל-set כדי לאפשר שינוי ב-BL/UI

        public OrderType OrderType { get; set; }

        public string? VerbalDescription { get; init; }        // נשאר init כיוון שזה נתון שמתקבל מההזמנה ולא אמור להשתנות בקלות

        public string? FullAddressOfTheOrder { get; set; }        // הופך ל-set כדי לאפשר שינוי כתובת בהזמנה


        public double Latitude { get; init; }        // נשאר init, כיוון שזו כתובת קבועה

        public double Longitude { get; init; }

        public double AirDistance { get; set; }

        public string? FullNameOfTheInviter { get; set; }        // הופך ל-set כדי לאפשר עדכון פרטי לקוח

        public string? OrderersPhoneNumber { get; set; }

        public OrderProperties OrderProperties { get; set; }
        public DateTime OrderOpenDate { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime MaximumDeliveryTime { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }
        public TimeSpan TimeLeftToCompleteOrder { get; set; }

        public List<DeliveryPerOrderInList>? deliveryPerOrderList { get; set; }
    }
}