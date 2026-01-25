using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Dal;

// DalXml - Order
internal class OrderImplementation : IOrder
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        List<Order> orders =
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);

        int nextId = Config.NextOrderId;

        item = item with { Id = nextId };

        orders.Add(item);

        XMLTools.SaveListToXMLSerializer(orders, Config.s_order_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement orderRootElem =
            XMLTools.LoadListFromXMLElement(Config.s_order_xml);

        XElement? orderElem =
            orderRootElem.Elements("Order")
            .FirstOrDefault(st => (int?)st.Element("Id") == id);

        if (orderElem is null)
            throw new DalDoesNotExistException($"Order with ID={id} does not exist");

        orderElem.Remove();

        XMLTools.SaveListToXMLElement(orderRootElem, Config.s_order_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(
            new List<Order>(),
            Config.s_order_xml
        );
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {
        List<Order> orders =
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);

        return orders.FirstOrDefault(it => it.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter)
    {
        List<Order> orders =
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);

        return orders.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        List<Order> orders =
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);

        return filter == null
            ? orders
            : orders.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        List<Order> orders =
            XMLTools.LoadListFromXMLSerializer<Order>(Config.s_order_xml);

        Order? order =
            orders.FirstOrDefault(it => it.Id == item.Id);

        if (order == null)
            throw new DalDoesNotExistException($"Order with ID={item.Id} does Not exist");

        orders.Remove(order);
        orders.Add(item);

        XMLTools.SaveListToXMLSerializer(orders, Config.s_order_xml);
    }
}
