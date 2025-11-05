// <summary>

using DO;

/// Order Entity represents a single order in the system,
/// containing information about the ordered product, customer,
/// delivery address, and time stamps.
/// </summary>
/// <param name="Id">Order unique ID number(auto number).</param>
/// <param name="OrderType">Order type.</param>
/// <param name="OrderNote">Verbal description of the order content.</param>
/// <param name="CustomerAddress">Full address of the customer.</param>
/// <param name="Latitude">Latitude coordinate of the delivery destination (double).</param>
/// <param name="Longitude">Longitude coordinate of the delivery destination (double).</param>
/// <param name="CustomerFullName">Full name of the customer who placed the order.</param>
/// <param name="CustomerPhone">Phone number of the customer.</param>
/// <param name="OrderProperties"> additional properties about the order.</param>
/// <param name="ShipDate">The date and time the order was shipped.</param>
/// <param name="DeliveryDate">The date and time the order was successfully delivered.</param>
/// <param name="OrderDate">The date and time when the order was placed.</param>
public record Order
(
    int Id,//כשנעשה את היישות תצורה להוסיף מספר רץ
    OrderType OrderType,
    string OrderNote,
    string CustomerAddress,
    double Latitude ,
    double Longitude,
    string CustomerFullName,
    string CustomerPhone,
    string? OrderProperties= null,
    DateTime? OrderDate = DateTime.Now
    )
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Order() : this(0,OrderType.TypeA,"","",0,0,"","") { }
    /// להחליף את הערך ברירת מחדל של המספר הרץ לממספר מהקונפיג
}

