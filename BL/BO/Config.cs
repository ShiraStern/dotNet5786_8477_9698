using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class Config
{
    public double? MaxRange { get; set; }
    public int ManagerID { get; set; }
    public DateTime Clock { get; set; } = DateTime.Now;
    public string? CompanyAddress { get; set; } = "בית הדפוס 9 ירושלים";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double AvgCarSpeed { get; set; } = 50;// in KM/H
    public double AvgMotorcycleSpeed { get; set; } = 60;// in KM/H
    public double AvgBicycleSpeed { get; set; } = 20;// in KM/H
    public  double AvgWalkingSpeed { get; set; } = 6;// in KM/H
    public TimeSpan MaxDeliveryDuration { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan DelayRiskTime { get; set; } = TimeSpan.FromDays(25);
    public TimeSpan InactivityThreshold { get; set; }

}
