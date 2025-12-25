using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
internal class CourierActivityCollection : IEnumerable
{
    static readonly IEnumerable<BO.FilterCouriersByProperty> s_enums =
        (Enum.GetValues(typeof(BO.FilterCouriersByProperty)) as IEnumerable<BO.FilterCouriersByProperty>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class Enums
{
}
