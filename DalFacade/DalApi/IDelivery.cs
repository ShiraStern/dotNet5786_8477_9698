using DO;
namespace DalApi;
/// <summary>
/// Defines the contract for managing delivery entities, including operations to create, read, update, and delete.
/// </summary>
/// <remarks>This interface provides methods for CRUD operations on delivery entities. It supports retrieving
/// individual entities by ID,  as well as bulk operations such as reading all entities or deleting all
/// entities.</remarks>
public interface IDelivery: ICrud<Delivery>
{
  
}
