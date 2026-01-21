using System;
using System.Collections;
using System.Collections.Generic;

namespace PL
{
    // רשימות עבור מסכי LIST
   

    internal class CourierActivityCollection : IEnumerable
    {
        static readonly IEnumerable<BO.FilterCouriersByProperty> courierEnums =
            (Enum.GetValues(typeof(BO.FilterCouriersByProperty)) as IEnumerable<BO.FilterCouriersByProperty>)!;

        public IEnumerator GetEnumerator() => courierEnums.GetEnumerator();
    }

   


    // רשימות עבור מסך ORDER

    internal class OrderTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.OrderType> types =
            (Enum.GetValues(typeof(BO.OrderType)) as IEnumerable<BO.OrderType>)!;

        public IEnumerator GetEnumerator() => types.GetEnumerator();
    }

    internal class OrderStatusCollectionForOrder : IEnumerable
    {
        static readonly IEnumerable<BO.OrderStatus> statuses =
            (Enum.GetValues(typeof(BO.OrderStatus)) as IEnumerable<BO.OrderStatus>)!;

        public IEnumerator GetEnumerator() => statuses.GetEnumerator();
    }
   

    //internal class OrederFilterCollection : IEnumerable
    //{
    //    static readonly IEnumerable<BO.filterOrdersByProperty> order_filter =
    //        (Enum.GetValues(typeof(BO.filterOrdersByProperty)) as IEnumerable<BO.filterOrdersByProperty>)!;

    //    public IEnumerator GetEnumerator() => order_filter.GetEnumerator();
    //}

    internal class ScheduleStatusCollection : IEnumerable
    {
        static readonly IEnumerable<BO.ScheduleStatus> schedules =
            (Enum.GetValues(typeof(BO.ScheduleStatus)) as IEnumerable<BO.ScheduleStatus>)!;

        public IEnumerator GetEnumerator() => schedules.GetEnumerator();
    }
    internal class OrderPropertiesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.OrderProperties> properties =
            (Enum.GetValues(typeof(BO.OrderProperties)) as IEnumerable<BO.OrderProperties>)!;

        public IEnumerator GetEnumerator() => properties.GetEnumerator();
    }


    // רשימות עבור מסך COURIER

    internal class DeliveryTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.DeliveryType> deliveries =
            (Enum.GetValues(typeof(BO.DeliveryType)) as IEnumerable<BO.DeliveryType>)!;

        public IEnumerator GetEnumerator() => deliveries.GetEnumerator();
    }
}
