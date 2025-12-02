
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class DeliveryImplementation : IDelivery
{
    public void Create(Delivery item)
    {
        int nextId = Config.NextDeliveryId;        // 1. קבלת ID חדש מ-Config. (מוודא ייחודיות ומקדם את המונה)
        item = item with { Id = nextId }; // עדכון הפריט ב-ID החדש
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);        // 2. טעינה, הוספה ושמירה
        deliveries.Add(item);        //-ID נוצר כרגע באופן ייחודי.
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_delivery_xml);
    }
    public void Delete(int id)
    {
        List<Delivery> Delivery = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);
        if (Delivery.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Delivery, Config.s_delivery_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Delivery>(), Config.s_delivery_xml);
    }

    public Delivery? Read(int id)
    {
        List<Delivery> delivery = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);
        return delivery.FirstOrDefault(it => it.Id == id);
    }

    public Delivery? Read(Func<Delivery, bool> filter)
    {
        List<Delivery> delivery = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);
        return delivery.FirstOrDefault(filter);
    }

    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        List<Delivery> delivery = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);
        return filter == null
            ? delivery
            : delivery.Where(filter);
    }

    public void Update(Delivery item)
    {
        List<Delivery> Delivery = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_delivery_xml);
        Delivery? delivery = Delivery.FirstOrDefault(it => it.Id == item.Id);
        if (Delivery == null)
            throw new DalDoesNotExistException($"Delivery with ID={item.Id} does Not exist");
        Delivery.Remove(delivery!);
        Delivery.Add(item);
        XMLTools.SaveListToXMLSerializer(Delivery, Config.s_delivery_xml);
    }
}
