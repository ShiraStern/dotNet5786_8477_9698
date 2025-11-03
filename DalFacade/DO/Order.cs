// <summary>
/// Order Entity represents a single order in the system,
/// containing information about the ordered product, customer,
/// delivery address, and time stamps.
/// </summary>
/// <param name="Id">Order unique ID number.</param>
/// <param name="CustomerName">Full name of the customer who placed the order.</param>
/// <param name="CustomerEmail">Email address of the customer.</param>
/// <param name="CustomerAddress">Full address of the customer (for billing/contact).</param>
/// <param name="Latitude">Latitude coordinate of the delivery destination (double).</param>
/// <param name="Longitude">Longitude coordinate of the delivery destination (double).</param>
/// <param name="DeliveryNotes">Additional notes or instructions for the delivery.</param>
/// <param name="OrderDate">The date and time the order was placed.</param>
/// <param name="ShipDate">The date and time the order was shipped.</param>
/// <param name="DeliveryDate">The date and time the order was successfully delivered.</param>
public record Order
(
    int Id,//כשנעשה את היישות תצורה להוסיף מספר רץ
    string CustomerName,
    string CustomerEmail,
    string CustomerAddress,
    double? Latitude = null,
    double? Longitude = null,
    string? DeliveryNotes = null,
    DateTime? OrderDate = null,
    DateTime? ShipDate = null,
    DateTime? DeliveryDate = null
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Order() : this(0, "", "", "") { }
}
