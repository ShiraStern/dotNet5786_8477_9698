
namespace DO;
public record Delivery
{
    int Id,
    int OrderId,
    int CourierId,
    DateTime? PickUpTime = null,
    DateTime? DeliveryTime = null
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Delivery() : this(0,0,0) { }
}
