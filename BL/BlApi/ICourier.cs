
using BO;

namespace BlApi;

public interface ICourier
{
    string Login(string userName, string password);
    IEnumerable<BO.CourierInList> GetCourierList(int id, bool isActive, sortCouriersByProperty? sortCouriersBy);
    BO.Courier GetDetails(int applicantId, int courierId);
    void UpdateDetails(int applicantId, BO.Courier boCourier);
    void Delete(int applicantId, int id);
    void AddCourier(int applicantId, BO.Courier boCourier);
}
