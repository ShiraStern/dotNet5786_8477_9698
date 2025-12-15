
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
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

    public DateTime GetClock() => AdminManager.Now; 
   // internal static DateTime Now { get => AdminManager.Now; } //stage 4

    public Config GetConfig()=> AdminManager.GetConfig();   


    public void InitializeDB()=>AdminManager.InitializeDB();
   

    public void ResetDB() => AdminManager.ResetDB(); 
 

    public void SetConfig(Config config)=> AdminManager.SetConfig(config);

}
