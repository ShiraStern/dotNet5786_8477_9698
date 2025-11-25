
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;



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
            Password: (string?)s.Element("Password"),
            Active: (bool?)s.Element("Active") ?? false,
            MaxDistance:(double?)s.Element("MaxDistance"),
            DeliveryType: s.ToEnumNullable<DO.DeliveryType>("DeliveryType") ?? DO.DeliveryType.Motorcycle,
            EmploymentStartDate: (DateTime)s.ToDateTimeNullable("EmploymentStartDate")
            );
    }

    public void Create(Courier item)
    {
        XElement? courierElem =
        XMLTools.LoadListFromXMLElement(Config.s_courier_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id);
        if (courierElem is not null)
            throw new DalAlreadyExistsException($"Order with ID={item.Id} already exists");
        courierElem!.Add(new XElement("Courier", item));
    }

    public void Delete(int id)
    {
        List<Courier> Courier = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_courier_xml);
        if (Courier.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Courier, Config.s_courier_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Courier>(), Config.s_courier_xml);
    }

    public Courier? Read(int id)
    {
        XElement? Courier =
        XMLTools.LoadListFromXMLElement(Config.s_courier_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return Courier is null ? null : getCourier(Courier);
    }

    public Courier? Read(Func<Courier, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_courier_xml).Elements().Select(s => getCourier(s)).FirstOrDefault(filter);
    }

    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Courier item)
    {
        throw new NotImplementedException();
    }
}
