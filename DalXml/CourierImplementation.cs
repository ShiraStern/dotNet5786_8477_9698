using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Dal;

internal class CourierImplementation : ICourier
{
    static Courier getCourier(XElement s)
    {
        return new DO.Courier
        (
            Id: s.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            FullName: (string?)s.Element("FullName") ?? "",
            Phone: (string?)s.Element("Phone") ?? "",
            Email: (string?)s.Element("Email") ?? "",
            Password: (string?)s.Element("Password") ?? "",
            Active: (bool?)s.Element("Active") ?? false,
            MaxDistance: (double?)s.Element("MaxDistance"),
            DeliveryType: s.ToEnumNullable<DO.DeliveryType>("DeliveryType") ?? DO.DeliveryType.Motorcycle,
            EmploymentStartDate: s.ToDateTimeNullable("EmploymentStartDate") ?? DateTime.Now
        );
    }

    // Creates an XElement from a Courier object for saving into XML
    private XElement createCourierElement(DO.Courier item)
    {
        return new XElement("Courier",
            new XElement("Id", item.Id),
            new XElement("FullName", item.FullName),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("Password", item.Password),
            new XElement("Active", item.Active),
            new XElement("MaxDistance", item.MaxDistance),
            new XElement("DeliveryType", item.DeliveryType),
            new XElement("EmploymentStartDate", item.EmploymentStartDate)
        );
    }

    // Adds a new Courier to the XML file
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        XElement couriersRootElem =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml);

        couriersRootElem.Add(createCourierElement(item));

        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);
    }

    // Delete a Courier by ID
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement couriersRootElem =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml);

        XElement? courierElem =
            couriersRootElem.Elements("Courier")
            .FirstOrDefault(st => (int?)st.Element("Id") == id);

        if (courierElem is null)
            throw new DalDoesNotExistException($"Courier with ID={id} does not exist");

        courierElem.Remove();

        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);
    }

    // Deletes all couriers
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement couriersRootElem =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml);

        couriersRootElem.RemoveAll();

        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);
    }

    // Reads a courier by ID
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id)
    {
        XElement? courier =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml)
            .Elements("Courier")
            .FirstOrDefault(st => (int?)st.Element("Id") == id);

        return courier is null ? null : getCourier(courier);
    }

    // Reads a courier by filter
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(Func<Courier, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_courier_xml)
            .Elements("Courier")
            .Select(s => getCourier(s))
            .FirstOrDefault(filter);
    }

    // Reads all couriers (with optional filter)
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        var list =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml)
            .Elements("Courier")
            .Select(getCourier);

        return filter == null ? list : list.Where(filter);
    }

    // Updates an existing courier
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        XElement couriersRootElem =
            XMLTools.LoadListFromXMLElement(Config.s_courier_xml);

        (couriersRootElem.Elements("Courier")
            .FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
            ?? throw new DalDoesNotExistException($"Courier with ID={item.Id} does Not exist"))
            .Remove();

        couriersRootElem.Add(createCourierElement(item));

        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);
    }
}
