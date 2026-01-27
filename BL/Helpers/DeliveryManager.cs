using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    internal static class DeliveryManager
    {
        private static IDal s_dal = Factory.Get; // stage 4

        internal static ObserverManager Observers = new(); // stage 5

        #region CRUD

        internal static void Create(DO.Delivery doDelivery)
        {
            lock (AdminManager.BlMutex)
                s_dal.Delivery.Create(doDelivery);

            Observers.NotifyListUpdated();
        }

        internal static void Delete(int deliveryID)
        {
            lock (AdminManager.BlMutex)
                s_dal.Delivery.Delete(deliveryID);

            Observers.NotifyItemUpdated(deliveryID);
            Observers.NotifyListUpdated();
            //OrderManager.Observers.NotifyItemUpdated(deliveryID);
            OrderManager.Observers.NotifyListUpdated();
        }

        internal static void Update(DO.Delivery doDelivery)
        {
            lock (AdminManager.BlMutex)
                s_dal.Delivery.Update(doDelivery);

             Observers.NotifyItemUpdated(doDelivery.Id);
            Observers.NotifyListUpdated();
        }

        internal static DO.Delivery? Read(int deliveryID)
        {
            lock (AdminManager.BlMutex)
                return s_dal.Delivery.Read(deliveryID);
        }

        #endregion


        #region Lists

        internal static IEnumerable<DO.Delivery>? GetList_DoDeliveriesByOrderId(int orderId)
        {
            lock (AdminManager.BlMutex)
            {
                return s_dal.Delivery.ReadAll()
                    .Where(c => c.OrderId == orderId)
                    .ToList();
            }
        }

        internal static DO.Delivery? GetLastDelivery(int orderID)
        {
            var list = GetList_DoDeliveriesByOrderId(orderID)?.ToList();

            if (list is null || !list.Any())
                return null;

            return list
                .OrderBy(d => d.DeliveryStartTime)
                .LastOrDefault();
        }

        internal static IEnumerable<BO.DeliveryPerOrderInList>? GetList_DeliveryPerOrderInList(int id)
        {
            var deliveries = GetList_DoDeliveriesByOrderId(id)?.ToList();

            if (deliveries is null || !deliveries.Any())
                return null;

            return deliveries
                .Where(x => x.DeliveryTermintionType != DeliveryTermintionType.DeliveredSeccessfully)
                .Select(x => ConvertTODeliveryPerOrderInList(x))
                .ToList();
        }


        internal static IEnumerable<DO.Delivery> GetList_DelivriesPerCourier(int id)
        {
            List<DO.Delivery> list;

            lock (AdminManager.BlMutex)
                list = s_dal.Delivery.ReadAll().ToList();

            return list.Where(d => d.CourierId == id);
        }

        #endregion


        #region Conversions

        internal static BO.DeliveryPerOrderInList ConvertTODeliveryPerOrderInList(DO.Delivery d)
        {
            DO.Courier courier;

            lock (AdminManager.BlMutex)
            {
                courier = s_dal.Courier.Read(d.CourierId)
                    ?? throw new BO.BlArgumentNullException(
                        $"Could not find courier {d.CourierId}");
            }

            return new BO.DeliveryPerOrderInList
            {
                DeliveryId = d.Id,
                courierId = d.CourierId,
                CourierName = courier.FullName,
                DeliveryType = (BO.DeliveryType)d.DeliveryType,
                DeliveryStart = d.DeliveryStartTime,
                DeliveryTerminationType =
                    (BO.DeliveryTerminationType)d.DeliveryTermintionType,
                DeliveryEndTime = d.DeliveryEndTime
            };
        }

        internal static List<BO.DeliveryPerOrderInList> ConvertList_ToDeliveryPerOrderInList(
            List<DO.Delivery> doDeliveries)
        {
            return doDeliveries
                .Select(d => ConvertTODeliveryPerOrderInList(d))
                .ToList();
        }

        #endregion
    }
}
