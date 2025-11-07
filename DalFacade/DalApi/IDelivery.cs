using DO;
namespace DalApi;
/// <summary>
/// Defines the contract for managing delivery entities, including operations to create, read, update, and delete.
/// </summary>
/// <remarks>This interface provides methods for CRUD operations on delivery entities. It supports retrieving
/// individual entities by ID,  as well as bulk operations such as reading all entities or deleting all
/// entities.</remarks>
public interface IDelivery
{
    void Create(Delivery item); //Creates new entity object in DAL
    Delivery? Read(int id); //Reads entity object by its ID 
    List<Delivery> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Delivery item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
