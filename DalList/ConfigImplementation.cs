using DalApi;

namespace Dal;


public class ConfigImplementation : IConfig
{
    public DateTime Clock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string? CompanyAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Latitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Longitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? MaxRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgCarSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgMotorcycleSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgDroneSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgWalkingSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan MaxDeliveryDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan DelayRiskTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan InactivityThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Reset()
    {
        Config.Reset(); 
    }
}
