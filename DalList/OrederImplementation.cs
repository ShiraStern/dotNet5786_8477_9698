using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal;

internal class OrderImplementation : IOrder
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        int newId = Config.NextOrderId;
        DataSource.Orders.Add(item with { Id = newId });
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Order? order = DataSource.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null)
            throw new InvalidOperationException("Order with the given Id does not exist.");

        DataSource.Orders.Remove(order);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Orders.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id) =>
        DataSource.Orders.Find(c => c.Id == id);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Orders.Select(item => item)
            : DataSource.Orders.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        Delete(item.Id);
        DataSource.Orders.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    Order? ICrud<Order>.Read(Func<Order, bool> filter)
    {
        return DataSource.Orders.FirstOrDefault(filter);
    }
}
