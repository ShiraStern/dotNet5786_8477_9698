using DalApi;  
using DO;

namespace Dal;

public class DeliveryImplementation : IDelivery
{
    public void Create(Delivery item)
    {
        if (item.Id != 0)
            throw new InvalidOperationException("Id must be zero when creating a new delivery.");
        int _id = Config.NextDeliveryId;
        DataSource.Deliveries.Add(item with { Id = _id });
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        Delivery? delivery = DataSource.Deliveries.FirstOrDefault(d => d.Id == id);
        if (delivery is null)
            throw new InvalidOperationException("Delivery with the given Id does not exist.");
        DataSource.Deliveries.Remove(delivery);
    }

    public void DeleteAll()
    {
        DataSource.Deliveries.Clear();
    }

    public Delivery? Read(int id)
    {
        if (DataSource.Deliveries.FirstOrDefault(d => d.Id == id) is not Delivery delivery)
            return null;    
        return delivery;
        throw new NotImplementedException();
    }

    public List<Delivery> ReadAll()
    {
        return new List<Delivery>(DataSource.Deliveries);
        throw new NotImplementedException();
    }

    public void Update(Delivery item)
    {
        if (DataSource.Deliveries.FirstOrDefault(d => d.Id == item.Id) is not Delivery)
            throw new InvalidOperationException("Delivery with the given Id does not exist.");
        //DataSource.Deliveries.Remove(item.)
        throw new NotImplementedException();
    }
}
