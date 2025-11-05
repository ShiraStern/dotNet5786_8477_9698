
namespace DalApi;
using DO;
/// <summary>
/// Defines the contract for managing entities, including operations to create, read, update, and
/// delete orders.
/// </summary>
/// <remarks>This interface provides methods for performing CRUD (Create, Read, Update, Delete) operations on entities. Implementations of this interface are responsible for interacting with the underlying data
/// store to manage order data.</remarks>
public interface IOrder
{
    void Create(Order item); //Creates new entity object in DAL
    Order? Read(int id); //Reads entity object by its ID 
    List<Order> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Order item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}
