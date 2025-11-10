using DalApi;  
using DO;

namespace Dal;

internal class DeliveryImplementation : IDelivery
{
    public void Create(Delivery item)
    {
        DataSource.Deliveries.Add(item with { Id = Config.NextDeliveryId });
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

   
    public Delivery? Read(int id) =>
        DataSource.Deliveries.Find(c => c.Id == id);


    //public List<Delivery> ReadAll()
    //{
    //    return new List<Delivery>(DataSource.Deliveries);
    //}
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Deliveries.Select(item => item)
            : DataSource.Deliveries.Where(filter);


    public void Update(Delivery item)
    {
        if (DataSource.Deliveries.FirstOrDefault(d => d.Id == item.Id) is not Delivery)
            throw new InvalidOperationException("Delivery with the given Id does not exist.");
        //DataSource.Deliveries.Remove(item.)
        throw new NotImplementedException();
    }

    Delivery? ICrud<Delivery>.Read(Func<Delivery, bool> filter)
    {
        throw new NotImplementedException();
    }
}
