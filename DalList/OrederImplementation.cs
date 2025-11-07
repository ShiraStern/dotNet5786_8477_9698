

using DalApi;
using DO;

namespace Dal;
public class OrederImplementation : IOrder
{
    public void Create(Order item)
    {
        DataSource.Orders.Add(item with { Id = Config.NextOrderId });
    }
    public void Delete(int id)
    {
        Order? order = DataSource.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null)
            throw new InvalidOperationException("Order with the given Id does not exist.");
        DataSource.Orders.Remove(order);
    }

    public void DeleteAll()
    {
        DataSource.Orders.Clear();
    }

     public Order? Read(int id) =>
        DataSource.Orders.FirstOrDefault(c => c.Id == id);

    public List<Order> ReadAll()
    {
        return new List<Order>(DataSource.Orders);
    }

    public void Update(Order item)
    {
        throw new NotImplementedException();
    }
}
