
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class OrederImplementation : IOrder
{
    public void Create(Order item)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
        if (Order.Exists(it => it.Id == item.Id))
            throw new DalAlreadyExistsException($"Order with ID={item.Id} already exists");
        Order.Add(item);
        XMLTools.SaveListToXMLSerializer(Order, Config.s_order_xml);
    }

    public void Delete(int id)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
        if (Order.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Order with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Order, Config.s_order_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Order>(), Config.s_order_xml);
    }

    public Order? Read(int id)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
        return Order.FirstOrDefault(it => it.Id==id);
    }

    public Order? Read(Func<Order, bool> filter)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
            return  Order.FirstOrDefault(filter);
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
        return filter == null
            ? Order
            : Order.Where(filter);
    }

    public void Update(Order item)
    {
        List<Order> Order = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);
        Order? order= Order.FirstOrDefault(it => it.Id == item.Id);
        if(order == null)
            throw new DalDoesNotExistException($"Order with ID={item.Id} does Not exist");
        Order.Remove(order);
        Order.Add(item);
        XMLTools.SaveListToXMLSerializer(Order, Config.s_order_xml);
    }
}
