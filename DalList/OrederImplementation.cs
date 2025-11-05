
namespace Dal;
using DalApi;
using DO;

public class OrederImplementation : IOrder
{
    public void Create(Order item)
    {
        if (item.Id != 0)
            throw new InvalidOperationException("Id must be zero when creating a new order.");
        int _id = Config.NextOrderId;
        DataSource.Orders.Add(item with { Id = _id });
        throw new NotImplementedException();
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

    public Order? Read(int id)
    {
        if (DataSource.Orders.FirstOrDefault(o => o.Id == id) is not Order order)
            return null;
        return order;
        throw new NotImplementedException();
    }

    public List<Order> ReadAll()
    {
        return new List<Order>(DataSource.Orders);
        throw new NotImplementedException();
    }

    public void Update(Order item)
    {
        throw new NotImplementedException();
    }
}
