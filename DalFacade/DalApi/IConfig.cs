
namespace DalApi;
using DO;

public interface IConfig
{
    DateTime Clock { get; set; }
    string? CompanyAddress { get; set; }
    double? Latitude { get; set; }
    double? Longitude { get; set; }
    double? MaxRange { get; set; }
    double AvgCarSpeed { get; set; }
    double AvgMotorcycleSpeed { get; set; }
    double AvgDroneSpeed { get; set; }
    double AvgWalkingSpeed { get; set; }
    TimeSpan MaxDeliveryDuration { get; set; }
    TimeSpan DelayRiskTime { get; set; }
    TimeSpan InactivityThreshold { get; set; }
    void Reset();
}


