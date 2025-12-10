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

    internal static void PeriodicCourierUpdates(DateTime oldClock, DateTime newClock)
    {
        throw new NotImplementedException();
    }

    internal static void SimulateCourseRegistrationAndGrade()
    {
        throw new NotImplementedException();
    }
}
