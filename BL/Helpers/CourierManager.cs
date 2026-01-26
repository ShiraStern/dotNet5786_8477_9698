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

    internal static void PeriodicCourierUpdates(DateTime oldClock, DateTime newClock)
    {
        List<DO.Courier> list;
        TimeSpan inactivityThreshold;

        // לקרוא DAL בצורה נעולה וקצרה
        lock (AdminManager.BlMutex) // stage 7
        {
            list = s_dal.Courier.ReadAll().ToList();
            inactivityThreshold = s_dal.Config.InactivityThreshold;
        }

        bool anyCourierUpdated = false; // stage 5
        var updatedCourierIds = new List<int>();

        foreach (var doCourier in list)
        {
            // אם שליח פעיל יותר מדי זמן ללא פעילות => להפוך ללא פעיל
            if (doCourier.Active &&
                AdminManager.Now - doCourier.EmploymentStartDate >= inactivityThreshold)
            {
                // עדכון DAL חייב להיות בתוך lock
                lock (AdminManager.BlMutex) // stage 7
                    s_dal.Courier.Update(doCourier with { Active = false });

                anyCourierUpdated = true;
                updatedCourierIds.Add(doCourier.Id);
            }
        }

        // Notifications חייבים להיות מחוץ ל-lock
        foreach (var id in updatedCourierIds)
            Observers.NotifyItemUpdated(id);

        if (anyCourierUpdated)
            Observers.NotifyListUpdated();
    }

    #region CRUD

    internal static void Create(DO.Courier doCourier)
    {
        lock (AdminManager.BlMutex) // stage 7
            s_dal.Courier.Create(doCourier);

        // stage 5 (מחוץ ל-lock)
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    internal static void Delete(int courierId)
    {
        lock (AdminManager.BlMutex) // stage 7
            s_dal.Courier.Delete(courierId);

        // stage 5 (מחוץ ל-lock)
        Observers.NotifyItemUpdated(courierId);
        Observers.NotifyListUpdated();
    }

    internal static void Update(DO.Courier doCourier)
    {
        lock (AdminManager.BlMutex) // stage 7
            s_dal.Courier.Update(doCourier);

        // stage 5 (מחוץ ל-lock)
        Observers.NotifyItemUpdated(doCourier.Id);
        Observers.NotifyListUpdated();
    }

    internal static DO.Courier? Read(int courierId)
    {
        lock (AdminManager.BlMutex) // stage 7
            return s_dal.Courier.Read(courierId);
    }

    internal static bool IsValidCourierId(int applicantId)
    {
        List<DO.Courier> list;
        lock (AdminManager.BlMutex) // stage 7
            list = s_dal.Courier.ReadAll().ToList();

        return list.Any(c => c.Id == applicantId);
    }

    internal static IEnumerable<DO.Courier> ReadAll()
    {
        lock (AdminManager.BlMutex) // stage 7
            return s_dal.Courier.ReadAll().ToList(); // להחזיר snapshot כדי שלא "יזוז" בזמן איטרציה
    }

    #endregion

    internal static BO.OrderInProgress? GetOrderInProgres(int courierId)
    {
        // DeliveryManager כבר אמור לנעול בפנים (לפי מה שסידרנו קודם),
        // ולכן כאן לא צריך lock נוסף סביב הקריאה אליו.
        var delivery = DeliveryManager.GetList_DelivriesPerCourier(courierId)
            .FirstOrDefault(x =>
                x.DeliveryTermintionType != DO.DeliveryTermintionType.None &&
                x.DeliveryTermintionType != DO.DeliveryTermintionType.DeliveredSeccessfully);

        if (delivery is null)
            return null;

        var order = OrderManager.GetOrderDetails(delivery.OrderId);
        return OrderManager.ConvertToOrderInProgress(delivery, order);
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

            // תיקון קטן: InTime צריך להיות "OnTime", לא "NotOnTime"
            NumOfDeliveriesInTime = GetNumOfDeliveriesOnTime(courier.Id),
            NumOfDeliveriesNotInTime = GetNumOfDeliveriesNotOnTime(courier.Id),

            OrderInProgress = GetOrderInProgres(courier.Id)
        };
        return boCourier;
    }

    internal static int? GetNumberOfDeliveriesInProcess(int id)
    {
        List<DO.Delivery> deliveries;
        lock (AdminManager.BlMutex) // stage 7
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == id &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.None);
    }

    internal static int GetNumOfDeliveriesNotOnTime(int id)
    {
        List<DO.Delivery> deliveries;
        lock (AdminManager.BlMutex) // stage 7
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == id &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
            d.DeliveryEndTime is not null &&
            d.DeliveryEndTime > d.DeliveryStartTime + AdminManager.MaxDeliveryDuration);
    }

    internal static int GetNumOfDeliveriesOnTime(int id)
    {
        List<DO.Delivery> deliveries;
        lock (AdminManager.BlMutex) // stage 7
            deliveries = s_dal.Delivery.ReadAll().ToList();

        return deliveries.Count(d =>
            d.CourierId == id &&
            d.DeliveryTermintionType == DO.DeliveryTermintionType.DeliveredSeccessfully &&
            d.DeliveryEndTime is not null &&
            d.DeliveryEndTime <= d.DeliveryStartTime + AdminManager.MaxDeliveryDuration);
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
        var oip = GetOrderInProgres(courier.Id);
        int? orderID = oip?.orderId;

        return new BO.CourierInList()
        {
            ID = courier.Id,
            FullName = courier.FullName,
            Active = courier.Active,
            DeliveryType = (BO.DeliveryType)courier.DeliveryType,
            EmploymentStartDate = courier.EmploymentStartDate,
            NumOfDeliveriesOnTime = GetNumOfDeliveriesOnTime(courier.Id),
            NumOfDeliveriesNotOnTime = GetNumOfDeliveriesNotOnTime(courier.Id),
            IdOfDeliveryInProcess = orderID
        };
    }

    private static void SimulateFinishDelivery()
    {
        try
        {
            List<DO.Delivery> activeDeliveries;

            lock (AdminManager.BlMutex) // stage 7
            {
                activeDeliveries = s_dal.Delivery.ReadAll()
                    .Where(d => d.DeliveryEndTime == null)
                    .ToList();
            }

            if (!activeDeliveries.Any())
                return;

            Random rand = new();
            var delivery = activeDeliveries[rand.Next(activeDeliveries.Count)];

            // close delivery (עדכון DAL נעול)
            DO.Delivery finished = delivery with
            {
                DeliveryEndTime = AdminManager.Now, // עדיף Now של הסימולטור
                DeliveryTermintionType = DO.DeliveryTermintionType.DeliveredSeccessfully,
                ActualDistance = delivery.ActualDistance ?? 1
            };

            lock (AdminManager.BlMutex) // stage 7
                s_dal.Delivery.Update(finished);

            // Notifications מחוץ ל-lock
                Observers.NotifyItemUpdated(delivery.CourierId);

            Observers.NotifyListUpdated();
        }
        catch
        {
            // simulation ignores failures
        }
    }

    /// <summary>
    /// שלד בטוח: אם יש אצלכם לוגיקה "לקיחת הזמנה" אמיתית ב-OrderManager/DeliveryManager,
    /// החליפו את הגוף לקריאה אליה.
    /// </summary>
    private static void SimulateTakeOrder()
    {
        try
        {
            // TODO: חברו ללוגיקה האמיתית שלכם (למשל: לבחור שליח פנוי + לבחור הזמנה ממתינה + ליצור Delivery)
            // כאן משאירים שלד שלא הורס ולא מניח מבנים שלא בטוח קיימים.
        }
        catch
        {
            // simulation ignores failures
        }
    }

    /// <summary>
    /// שלד בטוח: אם יש אצלכם לוגיקה "ביטול הזמנה" אמיתית ב-OrderManager,
    /// החליפו את הגוף לקריאה אליה.
    /// </summary>
    private static void SimulateCancelOrder()
    {
        try
        {
            // TODO: חברו ללוגיקה האמיתית שלכם
        }
        catch
        {
            // simulation ignores failures
        }
    }
}
