using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal;

internal class CourierImplementation : ICourier
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        // בדיקה אם השליח כבר קיים
        if (DataSource.Couriers.Any(c => c.Id == item.Id))
            throw new DalAlreadyExistsException($"Courier with the Id:{item.Id} already exists.");

        // הוספת השליח למאגר
        DataSource.Couriers.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Courier? courier = DataSource.Couriers.FirstOrDefault(c => c.Id == id);
        if (courier is null)
            throw new DalDoesNotExistException($"Courier with the Id:{id} does not exist.");

        DataSource.Couriers.Remove(courier);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
        => DataSource.Couriers.Clear();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id) =>
        DataSource.Couriers.Find(c => c.Id == id);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public List<Courier> ReadAll()
    {
        return new List<Courier>(DataSource.Couriers);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) //stage 2
         => filter == null
              ? DataSource.Couriers.Select(item => item)
              : DataSource.Couriers.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        Delete(item.Id);
        DataSource.Couriers.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    Courier? ICrud<Courier>.Read(Func<Courier, bool> filter)
    {
        return DataSource.Couriers.FirstOrDefault(filter);
    }
}
