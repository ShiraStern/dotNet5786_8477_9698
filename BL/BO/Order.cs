
using DalApi;
using DO;
using System.Net;
using System.Xml.Linq;

namespace BO
{
    public class Order
    {

        int ID { get; init; }
        DeliveryType kinddelivery { get; set; }
        string? VerbalDescription { get; init; }
        string FullAddressOfTheOrder { get; init; }
        double Latitude { get; init; }
        double Longitude { get; init; }
        double AirDistance { get; init; }
        string FullNameOfTheInviter { get; init; }
        string OrderersPhoneNumber { get; init; }
        OrderType features { get; init; }
        DateTime OrderOpeningTime { get; set; }
        DateTime? EstimatedDeliveryTime { get; set; }
        DateTime MaximumDeliveryTime { get; set; }
        OrderStatus OrderStatus { get; set; }
        ScheduleStatus ScheduleStatus { get; set; }
        TimeSpan TimeLeftToCompleteOrder { get; set; }
        List<DeliveryPerOrderInList>? deliveryPerOrderList{ get; set; }
    }
}
