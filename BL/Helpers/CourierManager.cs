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
        var list = s_dal.Courier.ReadAll().ToList();
        foreach (var doCoureir in list)
        {
            //if courier  is not active more than inactivitySpanTime 30 days
            //then courier should be automatically updated to 'not active'
            if (AdminManager.Now - doCoureir.EmploymentStartDate >= s_dal.Config.InactivityThreshold)
            {
                s_dal.Courier.Update(doCoureir with { Active = false });
            }
        }

    }
    internal static void Create(DO.Courier doCourier)
    {
        s_dal.Courier.Create(doCourier);
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }
    internal static void Delete(int courierId)
    {
        s_dal.Courier.Delete(courierId);
        Observers.NotifyItemUpdated(courierId);
        Observers.NotifyListUpdated();
    }
    internal static void Update(DO.Courier doCourier)
    {
        s_dal.Courier.Update(doCourier);
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }
    internal static DO.Courier? Read(int courierId)
    { 
            return s_dal.Courier.Read(courierId)?? null ;
    }

    internal static bool IsValidCourierId(int applicantId)
    { return s_dal.Courier.ReadAll().Any(c => c.Id == applicantId); }


    //internal static IDal GetDal()
    //{
    //    return s_dal;
    //}

    internal static IEnumerable<DO.Courier> ReadAll()
    {
       return s_dal.Courier.ReadAll();
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
        return s_dal.Delivery.ReadAll().
                    Where(c => c.CourierId == id &&
                    c.DeliveryTermintionType== DO.DeliveryTermintionType.None ).Count();
    }

    internal static int GetNumOfDeliveriesNotOnTime(int id)
    {
        return s_dal.Delivery.ReadAll().
            Where(c => c.CourierId == id &&
            c.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
            c.DeliveryEndTime > c.DeliveryStartTime + AdminManager.MaxDeliveryDuration).Count();
    }

    internal static int GetNumOfDeliveriesOnTime(int id)
    {
        return s_dal.Delivery.ReadAll().
            Where(c=> c.CourierId==id &&
            c.DeliveryTermintionType==DO.DeliveryTermintionType.DeliveredSeccessfully &&
            c.DeliveryEndTime<= c.DeliveryStartTime+AdminManager.MaxDeliveryDuration).Count();
    }

}
