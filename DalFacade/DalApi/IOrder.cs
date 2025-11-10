
namespace DalApi;
using DO;
/// <summary>
/// Defines the contract for managing entities, including operations to create, read, update, and
/// delete orders.
/// </summary>
/// <remarks>This interface provides methods for performing CRUD (Create, Read, Update, Delete) operations on entities. Implementations of this interface are responsible for interacting with the underlying data
/// store to manage order data.</remarks>
public interface IOrder: ICrud<Order> { }

