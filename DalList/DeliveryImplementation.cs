using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal;

internal class DeliveryImplementation : IDelivery
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        int newId = Config.NextDeliveryId;
        DataSource.Deliveries.Add(item with { Id = newId });
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Delivery? delivery = DataSource.Deliveries.FirstOrDefault(d => d.Id == id);
        if (delivery is null)
            throw new InvalidOperationException("Delivery with the given Id does not exist.");

        DataSource.Deliveries.Remove(delivery);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Deliveries.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id) =>
        DataSource.Deliveries.Find(c => c.Id == id);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Deliveries.Select(item => item)
            : DataSource.Deliveries.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        Delete(item.Id);
        DataSource.Deliveries.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    Delivery? ICrud<Delivery>.Read(Func<Delivery, bool> filter)
    {
        return DataSource.Deliveries.FirstOrDefault(filter);
    }
}
