using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5
    internal static void PeriodicCourierUpdates(DateTime oldClock, DateTime newClock)
    {
        throw new NotImplementedException();
    }

    
    

    internal static bool IsValidCourierId(int applicantId)
    { return s_dal.Courier.ReadAll().Any(c => c.Id == applicantId); }
       


    internal static void SimulateCourseRegistrationAndGrade()
    {
        throw new NotImplementedException();
    }

    internal static IDal GetDal()
    {
        return s_dal;
    }


    internal static DO.Courier ConvertToCourier(BO.Courier courier)
    {

        DO.Courier dalCourier = new DO.Courier()
        {
            Id = courier.ID,
            FullName = courier.FullName,
            Phone = courier.PhoneNember,
            Email = courier.Email,
            Password = courier.Password,
            Active = courier.Active,
            MaxDistance = courier.MaxDistance,
            DeliveryType = (DO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        return dalCourier;
    }

    internal static BO.Courier ConvertToCourier(DO.Courier courier)
    {

        BO.Courier dalCourier = new BO.Courier()
        {
            ID = courier.Id,
            FullName = courier.FullName,
            PhoneNember= courier.Phone,
            Email = courier.Email,
            Password = courier.Password,
            Active = courier.Active,
            MaxDistance = courier.MaxDistance,
            DeliveryType = (BO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate
        };
        return dalCourier;
    }
    internal static int? GetNumberOfDeliveriesInProcess(int id)
    {
        throw new NotImplementedException();
    }

    internal static int GetNumOfDeliveriesNotOnTime(int id)
    {
        throw new NotImplementedException();
    }

    internal static int GetNumOfDeliveriesOnTime(int id)
    {
        throw new NotImplementedException();
    }
}
