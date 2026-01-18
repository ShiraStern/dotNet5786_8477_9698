
//using System;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

//An entity that links an "order" to the "delivery" that it chose.
//A delivery person chose to make a delivery for the order.Includes a running and unique ID number of the linking entity.
//As well as an ID number of the order and the delivery person's ID number.
/// </summary>
/// <param name="Id">Delivery unique ID number(auto number).</param>
/// <param name="OrderId">Order entity running ID number (int).</param>
/// <param name="CourierId">The messenger's ID (int).</param>
/// <param name="DeliveryType">The type of shipment (DeliveryType ).</param>
/// <param name="DeliveryStartTime"> Delivery start time (DateTime).</param>
/// <param name="ActualDistance">actual distance ,can be null (double).</param>
/// <param name="DeliveryTermintionType">Delivery termination type,can be null .</param>
/// <param name="DeliveryEndTime">Delivery end time,can be null .</param>

public record Delivery
  (
    int Id,//כשנעשה את היישות תצורה להוסיף מספר רץ
    int OrderId,
    int CourierId,
    DeliveryType DeliveryType,
    DateTime DeliveryStartTime,
    double? ActualDistance = null,
    DeliveryTermintionType DeliveryTermintionType = DO.DeliveryTermintionType.None,
    DateTime? DeliveryEndTime = null
  )
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Delivery() : this(0, 0, 0, DeliveryType.None, DateTime.Now) { }
    
}