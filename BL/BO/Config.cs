using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class Config
{
    public double? MaxRange { get; set; }
    public int ManagerID { get; }
    DateTime Clock { get; set; } = DateTime.Now;
    string? CompanyAddress { get; set; } = "בית הדפוס 9 ירושלים";
    double? Latitude { get; set; }
    double? Longitude { get; set; }
    double AvgCarSpeed { get; set; } = 50;// in KM/H
    double AvgMotorcycleSpeed { get; set; } = 60;// in KM/H
    double AvgDroneSpeed { get; set; } = 20;// in KM/H
    double AvgWalkingSpeed { get; set; } = 6;// in KM/H
    TimeSpan MaxDeliveryDuration { get; set; } = TimeSpan.FromDays(30);
    TimeSpan DelayRiskTime { get; set; } = TimeSpan.FromDays(25);
    TimeSpan InactivityThreshold { get; set; }

}
