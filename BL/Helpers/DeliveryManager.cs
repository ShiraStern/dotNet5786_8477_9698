using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class DeliveryManager
    {
        private static IDal s_dal = Factory.Get; //stage 4

        internal static ObserverManager Observers = new(); //stage 5


        /// <summary>
        /// Retrieves all deliveries associated with the specified order identifier.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order for which to retrieve deliveries.</param>
        /// <returns>A list of <see cref="DO.Delivery"/> objects that are linked to the specified order.  Returns an empty list
        /// if no deliveries are found for the given order identifier.</returns>
        internal static List<DO.Delivery>? GetDoDeliveriesByOrderId(int orderId) // אנחנו צריכות להחזיר DO דליברי  לפי ה ORDER.ID
        {
            return s_dal.Delivery.ReadAll().Where(c => c.OrderId == orderId).ToList();
        }


        internal static List<BO.DeliveryPerOrderInList>? GetDeliveryPerOrderList(int id)
            => GetDoDeliveriesByOrderId(id).Select(static d => ConvertTODeliveryPerOrderInList(d)).ToList();
        

        internal static DO.Delivery? GetLastDelivery(IEnumerable<DeliveryPerOrderInList> list)
        {
            return s_dal.Delivery.Read(list.Last().DeliveryId) ??
                throw new BO.BlArgumentNullException("No deliveries found for the given order.");
        }


        internal static List< DO.Delivery>?  GetDelivriesPerCourier(int id)
        {
            var deliveryList = s_dal.Delivery.ReadAll();
            return deliveryList.Where(d => d.CourierId == id).ToList() ??
                throw new BO.BlArgumentNullException($"No deliveries found for the given courier with ID: {id}.");
        }


        /// <summary>
        /// Converts a <see cref="DO.Delivery"/> data object to a <see cref="BO.DeliveryPerOrderInList"/> business
        /// object.
        /// </summary>
        /// <param name="d">The delivery data object to convert. Must not be <see langword="null"/> and must reference a valid courier.</param>
        /// <returns>A <see cref="BO.DeliveryPerOrderInList"/> object containing the mapped delivery and courier information from
        /// the specified data object.</returns>
        /// <exception cref="BO.BlArgumentNullException">Thrown if the courier associated with the delivery cannot be found.</exception>
        internal static BO.DeliveryPerOrderInList ConvertTODeliveryPerOrderInList(DO.Delivery d)
        {
            var courier = s_dal.Courier.Read(d.CourierId) ??
                throw new BO.BlArgumentNullException($"Couldnt find the courier with ID:{d.OrderId} who handels the delivary{d.Id} ");
            return new BO.DeliveryPerOrderInList
            {
                DeliveryId = d.Id,
                courierId = d.CourierId,
                CourierName = courier.FullName,
                DeliveryType = (BO.DeliveryType)d.DeliveryType,
                DeliveryStart = d.DeliveryStartTime,
                DeliveryTerminationType = (BO.DeliveryTerminationType)d.DeliveryTermintionType,
                DeliveryEndTime = d.DeliveryEndTime
            };
        }



        /// <summary>
        /// Converts a list of data objects representing deliveries to a list of business objects for delivery per
        /// order.
        /// </summary>
        /// <param name="doDeliveries">The list of delivery data objects to convert. Cannot be null.</param>
        /// <returns>A list of <see cref="BO.DeliveryPerOrderInList"/> objects corresponding to the input deliveries. Returns an
        /// empty list if <paramref name="doDeliveries"/> is empty.</returns>
        internal static List<BO.DeliveryPerOrderInList> ConvertToDeliveryPerOrderInListList(List<DO.Delivery> doDeliveries)
        {
           
            return doDeliveries.Select(d => ConvertTODeliveryPerOrderInList(d)).ToList();
        }

    }

}
