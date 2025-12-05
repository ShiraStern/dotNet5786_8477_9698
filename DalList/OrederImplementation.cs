

using DalApi;
using DO;
using System.Reflection.Metadata.Ecma335;
//C: \Users\User\source\repos\dotNet5786_8477_9698\DalList\OrederImplementation.cs
namespace Dal;
internal class OrederImplementation : IOrder
{ public void Create(Order item)
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
        DataSource.Orders.Find(c => c.Id == id);

    //public List<Order> ReadAll()
    //    return new List<Order>(DataSource.Orders)
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Orders.Select(item => item)
            : DataSource.Orders.Where(filter);


    public void Update(Order item)
    {
        throw new NotImplementedException();
    }

    Order? ICrud<Order>.Read(Func<Order, bool> filter)
    {
        return DataSource.Orders.FirstOrDefault(filter);
        throw new NotImplementedException();
    }
}
