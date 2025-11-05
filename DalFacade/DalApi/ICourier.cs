


namespace DalApi;
using DO;

/// <summary>
/// Defines the contract for managing <see cref="Courier"/> entities, including operations to create, read, update, and
/// delete.
/// </summary>
/// <remarks>This interface provides methods for CRUD operations on <see cref="Courier"/> entities. 
/// Implementations of this interface are responsible for interacting with the underlying data access layer
/// (DAL).</remarks>
public interface ICourier
{
    void Create(Courier item); //Creates new entity object in DAL
    Courier? Read(int id); //Reads entity object by its ID 
    List<Courier> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Courier item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
