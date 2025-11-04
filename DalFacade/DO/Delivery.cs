
namespace DO;
public record Delivery
{
  ( 
    int Id,//כשנעשה את היישות תצורה להוסיף מספר רץ
    int OrderId,
    int CourierId,
    string CustomerAddress,
    DeliveryType DeliveryType,
    DateTime DeliveryStartTime,
    double? ActualDistance = null,
    string? DeliveryNotes = null,
    DateTime? OrderDate = null,
    DateTime? ShipDate = null,
    DateTime? DeliveryDate = null
  );
}
