

using DalApi;
using DO;

namespace Dal;
public class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        if(DataSource.Couriers.Any(c => c.Id == item.Id))
            throw new InvalidOperationException("Courier with the same Id already exists.");
        DataSource.Couriers.Add(item);
    }

    public void Delete(int id)
    {
        Courier? courier = DataSource.Couriers.FirstOrDefault(c => c.Id == id);
        if (courier is null)
            throw new InvalidOperationException("Courier with the given Id does not exist.");
        DataSource.Couriers.Remove(courier);
    }

    public void DeleteAll()
    {
        DataSource.Couriers.Clear();
    }

    public Courier? Read(int id)
    {
        if (DataSource.Couriers.FirstOrDefault(c => c.Id == id) is not Courier courier)
            return null;
        return courier;
        throw new NotImplementedException();
    }

    public List<Courier> ReadAll()
    {
        return new List<Courier>(DataSource.Couriers);
        throw new NotImplementedException();
    }

    public void Update(Courier item)
    {
        throw new NotImplementedException();
    }
}
