using DalApi;

namespace Dal;


internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public string? CompanyAddress
    {
        get => Config.CompanyAddress;
        set => Config.CompanyAddress = value;
    }
    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }
    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }
    public double? MaxRange
    {
        get => Config.MaxRange;
        set => Config.MaxRange = value;
    }
    public double AvgCarSpeed
    {
        get => Config.AvgCarSpeed;
        set => Config.AvgCarSpeed = value;
    }
    public double AvgMotorcycleSpeed
    {
        get => Config.AvgMotorcycleSpeed;
        set => Config.AvgMotorcycleSpeed = value;
    }
    public double AvgDroneSpeed
    {
        get => Config.AvgDroneSpeed;
        set => Config.AvgDroneSpeed = value;
    }
    public double AvgWalkingSpeed
    {
        get => Config.AvgWalkingSpeed;
        set => Config.AvgWalkingSpeed = value;
    }
    public TimeSpan MaxDeliveryDuration
    {
        get => Config.MaxDeliveryDuration;
        set => Config.MaxDeliveryDuration = value;
    }
    public TimeSpan DelayRiskTime
    {
        get => Config.DelayRiskTime;
        set => Config.DelayRiskTime = value;
    }
    public TimeSpan InactivityThreshold
    {
        get => Config.InactivityThreshold;
        set => Config.InactivityThreshold = value;
    }
    public void Reset()
    {
        Config.Reset(); 
    }
}
