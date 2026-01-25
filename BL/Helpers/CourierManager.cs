using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class CourierManager
{
    private static readonly IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5

    internal static void PeriodicCourierUpdates(DateTime oldClock, DateTime newClock)
    {
        var list = s_dal.Courier.ReadAll().ToList();

        bool anyCourierUpdated = false; // stage 5

        foreach (var doCourier in list)
        {
            // if courier is not active more than inactivity threshold
            // then courier should be automatically updated to 'not active'
            if (doCourier.Active &&
                AdminManager.Now - doCourier.EmploymentStartDate >= s_dal.Config.InactivityThreshold)
            {
                s_dal.Courier.Update(doCourier with { Active = false });
                anyCourierUpdated = true;

                // notify specific courier updated (stage 5)
                Observers.NotifyItemUpdated(doCourier.Id);
            }
        }

        // notify list updated only if something actually changed (stage 5)
        if (anyCourierUpdated)
            Observers.NotifyListUpdated();
    }

    internal static void Create(DO.Courier doCourier)
    {
        s_dal.Courier.Create(doCourier);

        // stage 5
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    internal static void Delete(int courierId)
    {
        s_dal.Courier.Delete(courierId);

        // stage 5
        Observers.NotifyItemUpdated(courierId);
        Observers.NotifyListUpdated();
    }

    internal static void Update(DO.Courier doCourier)
    {
        s_dal.Courier.Update(doCourier);

        // stage 5
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    internal static DO.Courier? Read(int courierId)
        => s_dal.Courier.Read(courierId);

    internal static bool IsValidCourierId(int applicantId)
        => s_dal.Courier.ReadAll().Any(c => c.Id == applicantId);

    internal static IEnumerable<DO.Courier> ReadAll()
        => s_dal.Courier.ReadAll();

   

    internal static BO.OrderInProgress? GetOrderInProgres(int courierId)
    {
        
        var delivery =
            DeliveryManager.GetList_DelivriesPerCourier(courierId)!
            .Where(x => x.DeliveryTermintionType != DO.DeliveryTermintionType.None
            || x.DeliveryTermintionType != DO.DeliveryTermintionType.DeliveredSeccessfully).FirstOrDefault();
        if (delivery is null)
            return null;
        var order= OrderManager.GetOrderDetails(delivery.OrderId);
        return  OrderManager. ConvertToOrderInProgress(delivery, order);



    }


    internal static DO.Courier ConvertToCourier(BO.Courier courier)
    {
        DO.Courier dalCourier = new()
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
        BO.Courier boCourier = new()
        {
            ID = courier.Id,
            FullName = courier.FullName,
            PhoneNember = courier.Phone,
            Email = courier.Email,
            Password = courier.Password,
            Active = courier.Active,
            MaxDistance = courier.MaxDistance,
            DeliveryType = (BO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate,
            NumOfDeliveriesInTime = GetNumOfDeliveriesNotOnTime(courier.Id),
            NumOfDeliveriesNotInTime = GetNumOfDeliveriesNotOnTime(courier.Id),
            OrderInProgress = GetOrderInProgres(courier.Id)

        };
        return boCourier;
    }

    internal static int? GetNumberOfDeliveriesInProcess(int id)
    {
        return s_dal.Delivery.ReadAll()
            .Where(d => d.CourierId == id &&
                        d.DeliveryTermintionType == DO.DeliveryTermintionType.None)
            .Count();
    }

    internal static int GetNumOfDeliveriesNotOnTime(int id)
    {
        return s_dal.Delivery.ReadAll()
            .Where(d => d.CourierId == id &&
                        d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
                        d.DeliveryEndTime > d.DeliveryStartTime + AdminManager.MaxDeliveryDuration)
            .Count();
    }

    internal static int GetNumOfDeliveriesOnTime(int id)
    {
        return s_dal.Delivery.ReadAll()
            .Where(d => d.CourierId == id &&
                        d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
                        d.DeliveryEndTime <= d.DeliveryStartTime + AdminManager.MaxDeliveryDuration)
            .Count();
    }

    // -------------------- Simulation --------------------

    internal static async Task SimulateAsync()
    {
        // small delay so simulator won't block (fine)
        await Task.Delay(10);

        Random rand = new();
        int action = rand.Next(3);

        switch (action)
        {
            case 0:
                SimulateTakeOrder();
                break;
            case 1:
                SimulateFinishDelivery();
                break;
            case 2:
                SimulateCancelOrder();
                break;
        }
    }


    internal static BO.CourierInList ConvertToCourierInList(DO.Courier courier)
    {
        int? orderID = GetOrderInProgres(courier.Id) is null 
            ? null : GetOrderInProgres(courier.Id)!.orderId;
        return new BO.CourierInList()
        {
            ID = courier.Id,
            FullName = courier.FullName,
            Active = courier.Active,
            DeliveryType = (BO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate,
            NumOfDeliveriesOnTime = CourierManager.GetNumOfDeliveriesOnTime(courier.Id),
            NumOfDeliveriesNotOnTime = CourierManager.GetNumOfDeliveriesNotOnTime(courier.Id),
            IdOfDeliveryInProcess = orderID
         };
    }

    private static void SimulateFinishDelivery()
    {
        try
        {
            // active deliveries (not finished yet)
            var activeDeliveries = s_dal.Delivery.ReadAll()
                .Where(d => d.DeliveryEndTime == null)
                .ToList();

            if (!activeDeliveries.Any())
                return;

            Random rand = new();
            var delivery = activeDeliveries[rand.Next(activeDeliveries.Count)];

            // close delivery
            DO.Delivery finished = delivery with
            {
                DeliveryEndTime = DateTime.Now,
                DeliveryTermintionType = DO.DeliveryTermintionType.DeliveredSeccessfully,
                ActualDistance = delivery.ActualDistance ?? 1
            };

            s_dal.Delivery.Update(finished);

            // IMPORTANT FIX:
            // notify by COURIER id for courier observers, not by OrderId.
            if (delivery is not null)
            {
                Observers.NotifyItemUpdated(delivery.CourierId);
            }
            Observers.NotifyListUpdated();

            // Note:
            // If you have OrderManager observers and your UI expects order list/details to refresh,
            // OrderManager should notify its own observers inside its update methods.
            // Here we only ensured courier UI gets refreshed correctly.
        }
        catch
        {
            // simulation ignores failures
        }
    }


    private static void SimulateTakeOrder()
    {

    }

    private static void SimulateCancelOrder()
    { 

    }

}
