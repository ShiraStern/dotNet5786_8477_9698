namespace Dal;
//C:\Users\User\source\repos\dotNet5786_8477_9698\DalXml\Config.cs
internal class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_courier_xml = "couriers.xml";
    internal const string s_order_xml = "orders.xml";
    internal const string s_delivery_xml = "delivery.xml";
    //...	

    internal static int NextOrderId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
    }
  
    internal static int NextDeliveryId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
    }

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static int ManagerID 
    {
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerID");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerID", value=216318477);
    }
    internal static string ManagerPassword
    {
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPassword");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPassword", value="1234");
    }
    internal static string? CompanyAddress 
    {
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value= $"בית הדפוס 9 ירושלים");
    }
    internal static double? Latitude 
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Latitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value= 31.785824401794827);
    }
    internal static double? Longitude
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Longitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value = 35.18925479816518);
    }
    internal static double? MaxRange
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "MaxRange");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "MaxRange", value/* = 290*/);
    }  // in KM,only in Israel
    internal static double AvgCarSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
    } // in KM/H
    internal static double AvgMotorcycleSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotorcycleSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotorcycleSpeed", value);
    } // in KM/H
    internal static double AvgBicycleSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
    } // in KM/H
    internal static double AvgWalkingSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
    }// in KM/H
    internal static TimeSpan MaxDeliveryDuration /*{ get; set; } = TimeSpan.FromDays(30);*/
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "MaxDeliveryDuration");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "MaxDeliveryDuration", value );
    }
    internal static TimeSpan DelayRiskTime
        {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "DelayRiskTime");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "DelayRiskTime", value );
    }
    internal static TimeSpan InactivityThreshold
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "InactivityThreshold");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "InactivityThreshold", value);
    }
    public const int MIN_ID = 200000000;
    public const int MAX_ID = 400000000;

    internal static void Reset()
    {
        NextOrderId = 1000;
        NextDeliveryId = 100;
        Clock = DateTime.Now;
        CompanyAddress = null;
        Latitude = null;
        Longitude = null;
        MaxRange = null;
        AvgCarSpeed = 50;
        AvgBicycleSpeed = 60;
        AvgMotorcycleSpeed = 20;
        AvgWalkingSpeed = 6;
        MaxDeliveryDuration = TimeSpan.FromDays(30);
        DelayRiskTime = TimeSpan.FromDays(25);
        InactivityThreshold = TimeSpan.FromDays(30);
    }

}
