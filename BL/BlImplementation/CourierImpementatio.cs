
namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;

internal class CourierImpementatio : ICourier
{
    public void AddCourier(int applicantId, Courier boCourier)
    {
        throw new NotImplementedException();
    }

    public void Delete(int applicantId, int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CourierInList> GetCourierList(int id, bool isActive, sortCouriersByProperty? sortCouriersBy)
    {
        throw new NotImplementedException();
    }

    public Courier GetDetails(int applicantId, int courierId)
    {
        throw new NotImplementedException();
    }

    public string Login(string userName, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateDetails(int applicantId, Courier boCourier)
    {
        throw new NotImplementedException();
    }
}
