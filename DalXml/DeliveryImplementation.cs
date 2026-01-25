using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dal;

internal class DeliveryImplementation : IDelivery
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        int nextId = Config.NextDeliveryId; // קבלת ID חדש
        item = item with { Id = nextId };

        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        deliveries.Add(item);

        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_delivery_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        if (deliveries.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Delivery with ID={id} does Not exist");

        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_delivery_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(
            new List<Delivery>(),
            Config.s_delivery_xml
        );
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {
        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        return deliveries.FirstOrDefault(it => it.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        return deliveries.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        return filter == null
            ? deliveries
            : deliveries.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        List<Delivery> deliveries =
            XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);

        Delivery? delivery =
            deliveries.FirstOrDefault(it => it.Id == item.Id);

        if (delivery == null)
            throw new DalDoesNotExistException($"Delivery with ID={item.Id} does Not exist");

        deliveries.Remove(delivery);
        deliveries.Add(item);

        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_delivery_xml);
    }
}
