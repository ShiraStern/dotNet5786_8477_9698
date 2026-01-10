using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
internal class OrderStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.filterOrdersByProperty> oreder_enums =
        (Enum.GetValues(typeof(BO.filterOrdersByProperty)) as IEnumerable<BO.filterOrdersByProperty>)!;

    public IEnumerator GetEnumerator() => oreder_enums.GetEnumerator();
}
internal class CourierActivityCollection : IEnumerable
{
    static readonly IEnumerable<BO.FilterCouriersByProperty> courier_enums =
        (Enum.GetValues(typeof(BO.FilterCouriersByProperty)) as IEnumerable<BO.FilterCouriersByProperty>)!;

    public IEnumerator GetEnumerator() => courier_enums.GetEnumerator();
}


internal class Enums
{
}
