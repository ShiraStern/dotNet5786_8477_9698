using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers;

internal static class CourierManager
{
    private static readonly IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5
    private static readonly AsyncMutex s_periodicMutex = new(); // stage 7
    private static readonly AsyncMutex s_simulationMutex = new(); // stage 7
    private static readonly Random s_rand = new();

    // -------------------- Periodic Updates --------------------

    internal static void PeriodicCourierUpdates(DateTime oldClock, DateTime newClock)
    {
        if (s_periodicMutex.CheckAndSetInProgress())
            return;
        try
        {
            List<DO.Courier> list;
        TimeSpan inactivityThreshold;

        lock (AdminManager.BlMutex) // stage 7
        {
            list = s_dal.Courier.ReadAll().ToList();
            inactivityThreshold = s_dal.Config.InactivityThreshold;
        }

        bool anyCourierUpdated = false;
        var updatedCourierIds = new List<int>();

        foreach (var doCourier in list)
        {
            if (doCourier.Active &&
                AdminManager.Now - doCourier.EmploymentStartDate >= inactivityThreshold)
            {
                lock (AdminManager.BlMutex)
                    s_dal.Courier.Update(doCourier with { Active = false });

                anyCourierUpdated = true;
                updatedCourierIds.Add(doCourier.Id);
            }
        }

        foreach (var id in updatedCourierIds)
            Observers.NotifyItemUpdated(id);

        if (anyCourierUpdated)
            Observers.NotifyListUpdated();
        }
        finally
{
            s_periodicMutex.UnsetInProgress();
        }

    }

    #region CRUD

    internal static void Create(DO.Courier doCourier)
    {
        lock (AdminManager.BlMutex)
            s_dal.Courier.Create(doCourier);

        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    internal static void Delete(int courierId)
    {
        lock (AdminManager.BlMutex)
            s_dal.Courier.Delete(courierId);

        Observers.NotifyItemUpdated(courierId);
        Observers.NotifyListUpdated();
    }

    internal static void Update(DO.Courier doCourier)
    {
        lock (AdminManager.BlMutex)
            s_dal.Courier.Update(doCourier);

        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }
    internal static void Update(BO.Courier doCourier)
    {
        lock (AdminManager.BlMutex)
            s_dal.Courier.Update(ConvertToCourier( doCourier));


        Observers.NotifyItemUpdated(doCourier.ID);
        Observers.NotifyListUpdated();
    }

    internal static DO.Courier? Read(int courierId)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Courier.Read(courierId);
    }

    internal static bool IsValidCourierId(int applicantId)
    {
        List<DO.Courier> list;

        lock (AdminManager.BlMutex)
            list = s_dal.Courier.ReadAll().ToList();

        return list.Any(c => c.Id == applicantId);
    }

    internal static IEnumerable<DO.Courier> ReadAll()
    {
        lock (AdminManager.BlMutex)
            return s_dal.Courier.ReadAll().ToList();
    }

    #endregion

    // -------------------- Orders --------------------

    internal static BO.OrderInProgress? GetOrderInProgres(int courierId)
    {
        var delivery = DeliveryManager
            .GetList_DelivriesPerCourier(courierId)
            .FirstOrDefault(d =>
                d.DeliveryTermintionType == DO.DeliveryTermintionType.None);

        if (delivery is null)
            return null;

        var order = OrderManager.GetOrderDetails(delivery.OrderId);

        return OrderManager.ConvertToOrderInProgress(delivery, order);
    }

    // -------------------- Conversions --------------------

    internal static DO.Courier ConvertToCourier(BO.Courier courier)
    {
        return new DO.Courier
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
    }

    internal static BO.Courier ConvertToCourier(DO.Courier courier)
    {
        return new BO.Courier
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

            NumOfDeliveriesInTime = GetNumOfDeliveriesOnTime(courier.Id),
            NumOfDeliveriesNotInTime = GetNumOfDeliveriesNotOnTime(courier.Id),

            OrderInProgress = GetOrderInProgres(courier.Id)
        };
    }

    // -------------------- Statistics --------------------

    internal static int? GetNumberOfDeliveriesInProcess(int courierId)
    {
        List<DO.Delivery> deliveries;

        lock (AdminManager.BlMutex)
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == courierId &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.None);
    }

    internal static int GetNumOfDeliveriesNotOnTime(int id)
    {
        List<DO.Delivery> deliveries;

        lock (AdminManager.BlMutex)
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == id &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
            d.DeliveryEndTime is not null &&
            d.DeliveryEndTime >
            d.DeliveryStartTime + AdminManager.MaxDeliveryDuration);
    }

    internal static int GetNumOfDeliveriesOnTime(int id)
    {
        List<DO.Delivery> deliveries;

        lock (AdminManager.BlMutex)
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == id &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
            d.DeliveryEndTime is not null &&
            d.DeliveryEndTime <=
            d.DeliveryStartTime + AdminManager.MaxDeliveryDuration);
    }

    // -------------------- Simulation --------------------

    internal static async Task SimulateAsync() // stage 7
    {
        // אם סימולציה קודמת עדיין רצה → יוצאים
        if (s_simulationMutex.CheckAndSetInProgress())
            return;

        try
        {
            await Task.Delay(10); // לא לחסום Thread

            int action = s_rand.Next(3);

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
        finally
        {
            // תמיד לשחרר
            s_simulationMutex.UnsetInProgress();
        }
    }


    internal static BO.CourierInList ConvertToCourierInList(DO.Courier courier)
    {
        var oip = GetOrderInProgres(courier.Id);

        return new BO.CourierInList
        {
            ID = courier.Id,
            FullName = courier.FullName,
            Active = courier.Active,
            DeliveryType = (BO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate,

            NumOfDeliveriesOnTime = GetNumOfDeliveriesOnTime(courier.Id),
            NumOfDeliveriesNotOnTime = GetNumOfDeliveriesNotOnTime(courier.Id),

            IdOfDeliveryInProcess = oip?.orderId
        };
    }

    // -------------------- Simulation Helpers --------------------

    private static void SimulateFinishDelivery()
    {
        try
        {
            List<DO.Delivery> activeDeliveries;

            lock (AdminManager.BlMutex)
            {
                activeDeliveries = s_dal.Delivery.ReadAll()
                    .Where(d => d.DeliveryEndTime == null)
                    .ToList();
            }

            if (!activeDeliveries.Any())
                return;

            var delivery = activeDeliveries[s_rand.Next(activeDeliveries.Count)];

            DO.Delivery finished = delivery with
            {
                DeliveryEndTime = AdminManager.Now,
                DeliveryTermintionType = DO.DeliveryTermintionType.DeliveredSeccessfully,
                ActualDistance = delivery.ActualDistance ?? 1
            };

            lock (AdminManager.BlMutex)
            {
                s_dal.Delivery.Update(finished);
            }

            Observers.NotifyItemUpdated(delivery.CourierId);
            Observers.NotifyListUpdated();
 
        }
        catch
        {
            // ignore simulation errors
        }
    }

    private static void SimulateTakeOrder()
    {
        try
        {
            // TODO: חיבור ללוגיקה אמיתית אם צריך
        }
        catch
        {
        }
    }

    private static void SimulateCancelOrder()
    {
        try
        {
            // TODO: חיבור ללוגיקה אמיתית אם צריך
        }
        catch
        {
        }
    }
}
