namespace Dal;

internal class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_courier_xml = "courier.xml";
    internal const string s_order_xml = "order.xml";
    internal const string s_delivery_xml = "delivery.xml";
    //...	

    internal static int NextCourseId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCourseId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCourseId", value);
    }

    //...	

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static void Reset()
    {
        NextCourseId = 1000;
            //...
        Clock = DateTime.Now;
        //...
    }

}
