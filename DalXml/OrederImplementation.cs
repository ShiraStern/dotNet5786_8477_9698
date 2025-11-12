
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class OrederImplementation : IOrder
{
    public void Create(Order item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        List<> Courses = XMLTools.LoadListFromXMLSerializer<Course>(Config.s_courses_xml);
        if (Courses.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Courses, Config.);
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Order? Read(int id)
    {
        throw new NotImplementedException();
    }

    public Order? Read(Func<Order, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Order item)
    {
        throw new NotImplementedException();
    }
}
