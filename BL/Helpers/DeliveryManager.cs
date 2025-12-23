using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class DeliveryManager
    {
        private static IDal s_dal = Factory.Get; //stage 4

        internal static ObserverManager Observers = new(); //stage 5
        internal static List<DO.Delivery>?  GetDoDeliveriesByOrderId(int orderId) // אנחנו צריכות להחזיר DO דליברי  לפי ה ORDER.ID
        {
            return s_dal.Delivery.ReadAll().Where(c => c.OrderId == orderId).ToList();
        }
    }
    
}
