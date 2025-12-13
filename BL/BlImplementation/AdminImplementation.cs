
namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    public void ForwardClock(TimeUnit unit)
    {
        switch (unit)
        {
            case TimeUnit.Minute: AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1)); break;
            case TimeUnit.Hour: AdminManager.UpdateClock(AdminManager.Now.AddHours(1)); break;
            case TimeUnit.Day: AdminManager.UpdateClock(AdminManager.Now.AddDays(1)); break;
            case TimeUnit.Month: AdminManager.UpdateClock(AdminManager.Now.AddMonths(1)); break;
            case TimeUnit.Year: AdminManager.UpdateClock(AdminManager.Now.AddYears(1)); break;
        }
    }


    public DateTime GetClock()
    {
        throw new NotImplementedException();
    }

    public Config GetConfig()
    {
        throw new NotImplementedException();
    }

    public void InitializeDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public void SetConfig(Config config)
    {
        throw new NotImplementedException();
    }
}
