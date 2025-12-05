
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
//C:\Users\User\source\repos\dotNet5786_8477_9698\DalXml\OrederImplementation.cs
internal class OrederImplementation : IOrder
{
    public void Create(Order item)
    {
        List<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);        //  טעינת הרשימה
        int nextId = Config.NextOrderId;        //  קבלת ID חדש מ-Config.
        item = item with { Id = nextId };        //  עדכון האובייקט שנכנס למתודה עם ה-ID החדש.
        orders.Add(item);        //  הוספה ושמירה
        XMLTools.SaveListToXMLSerializer(orders, Config.s_order_xml);
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
