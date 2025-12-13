
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Password: (string?)s.Element("Password")?? "",
            Active: (bool?)s.Element("Active") ?? false,
            MaxDistance:(double?)s.Element("MaxDistance"),
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
    public void Create(Courier item)
    {
        //הitem מגיע כבר ע תעודת זהות
        //int nextId = Config.NextCourierId;
        //item = item with { Id = nextId };        
        XElement couriersRootElem = XMLTools.LoadListFromXMLElement(Config.s_courier_xml);        //  טעינת קובץ ה-XML (שורש הנתונים).
        couriersRootElem.Add(createCourierElement(item));        //  הוספת האלמנט החדש שנוצר מהאובייקט (באמצעות createCourierElement).
        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);        // שמירת ה-XElement המעודכן בחזרה לקובץ XML.
    }
    // מוחקת שליח לפי קוד מזהה
    // Deletes a Courier by ID
    public void Delete(int id)
    {
        XElement couriersRootElem = XMLTools.LoadListFromXMLElement(Config.s_courier_xml);        // טעינת קובץ ה-XML
        XElement? courierElem = couriersRootElem.Elements("Courier")        //מציאת האלמנט למחיקה
                                                .FirstOrDefault(st => (int?)st.Element("Id") == id);
        if (courierElem is null)
        {throw new DalDoesNotExistException($"Courier with ID={id} does not exist");}
        courierElem.Remove();        // מחיקת האלמנט
        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);        // שמירת ה-XElement המעודכן
    }
    // מוחקת את כל השליחים מהקובץ
    // Deletes all couriers from the XML file
    public void DeleteAll()
    {
        XElement couriersRootElem = XMLTools.LoadListFromXMLElement(Config.s_courier_xml);
        couriersRootElem.RemoveAll(); // מחיקת כל הצאצאים של אלמנט השורש
        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);
    }
    // קוראת שליח לפי מזהה ומחזירה אובייקט או null
    // Reads a courier by ID and returns a Courier or null
    public Courier? Read(int id)
    {
        XElement? Courier =
        XMLTools.LoadListFromXMLElement(Config.s_courier_xml).Elements("Courier").FirstOrDefault(st => (int?)st.Element("Id") == id);
        return Courier is null ? null : getCourier(Courier);
    }
    // קוראת שליח לפי תנאי (פונקציית פילטר)
    // Reads a courier that matches a given filter function
    public Courier? Read(Func<Courier, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_courier_xml).Elements().Select(s => getCourier(s)).FirstOrDefault(filter);
    }

    // קוראת את כל השליחים, עם או בלי פילטר
    // Reads all couriers, optionally using a filter function
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        var list = XMLTools.LoadListFromXMLElement(Config.s_courier_xml)
                           .Elements("Courier")
                           .Select(getCourier);

        return filter == null ? list : list.Where(filter);
    }

    // מעדכנת שליח קיים בקובץ הXML
    // Updates an existing courier in the XML file
    public void Update(Courier item)
    {
        XElement couriersRootElem = XMLTools.LoadListFromXMLElement(Config.s_courier_xml);        // טעינת קובץ ה-XML
        (couriersRootElem.Elements("Courier").FirstOrDefault(st => (int?)st.Element("Id") == item.Id)        // מציאת האלמנט הישן וסילוקו. אם לא נמצא, זורקים חריגה.
            ?? throw new DalDoesNotExistException($"Courier with ID={item.Id} does Not exist"))
            .Remove();
        couriersRootElem.Add(createCourierElement(item));        // הוספת האלמנט המעודכן
        XMLTools.SaveListToXMLElement(couriersRootElem, Config.s_courier_xml);        //  שמירת ה-XElement מעודכן בחזרה לקובץ XML

    }
}
