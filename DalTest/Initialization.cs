namespace Dal;
using DalApi;
using DO;
public static class Initialization
{
    private static ICourier? s_dalCourier; //stage 1
    private static IDelivery? s_dalDelivery; //stage 1
    private static IOrder? s_dalOrder; //stage 1
    private static IConfig? s_dalConfig; //stage 1



    private static readonly Random s_random = new();
    private static void createCouriers()
    {
        string[] couriersNames =
            {"David Cohen", "Yael Levi", "Avi Mizrahi", "Noa Rosen", "Eli Ben-David", "Dana Shapiro", "Yossi Azulay", "Tamar Katz", "Itay Goldstein", "Lior Peretz", "Roni Bar", "Shiran Malka", "Nadav Friedman", "Gal Cohen", "Ori Hadad", "Maya Regev", "Yonatan Amir", "Eden Sharon", "Alon Biton", "Hila Segal" };


        foreach (var name in couriersNames)
        {
            int id;
            do
                id = s_random.Next(200000000, 400000000);
            while (s_dalCourier!.Read(id) is not null);
            Courier courier = new()
            {
                Id = id,
                FullName = name,
                Phone = $"05{s_random.Next(0, 9)}-{s_random.Next(1000000, 9999999)}",
                Email = $"{name.Replace(" ", "")}@gmail.com",
                Password = $"{name.Length % 97}{s_random.Next(1000, 9999)}",
                Active = s_random.Next(0, 2) == 1,
                MaxDistance = s_random.Next(40, (int)(s_dalConfig!.MaxRange ?? 297)),
                DeliveryType = (DeliveryType)s_random.Next(0, 4),
                EmploymentStartDate = s_dalConfig!.Clock.AddYears(-s_random.Next(0, 10))
            };
            s_dalCourier!.Create(courier);
        }
    }
    private static void createOrders()
    {
        string[] customerAddresses =
        {
            "הרצל 12 תל אביב",
            "שדרות רוטשילד 45 תל אביב",
            "אלנבי 78 תל אביב",
            "בן יהודה 23 תל אביב",
            "סוקולוב 56 רמת גן",
            "דרך יהודית 34 רמת גן",
            "דרך בן גוריון 12 ראשון לציון",
            "ויצמן 78 ראשון לציון",
            "דרך ההגנה 9 פתח תקווה",
            "דגל ראובן 15 פתח תקווה",
            "דרך חברון 45 ירושלים",
            "דרך בית לחם 12 ירושלים",
            "רחוב שמאי 22 ירושלים",
            "רחוב המלך ג'ורג' 78 ירושלים",
            "אבן גבירול 15 תל אביב",
            "דיזנגוף 47 תל אביב",
            "העצמאות 6 רעננה",
            "דרך ירושלים 89 רעננה",
            "הס 21 חיפה",
            "דרך הכרמל 44 חיפה",
            "חטיבת הנגב 18 באר שבע",
            "שדרות נחל הבשור 7 באר שבע",
            "ויצמן 33 חיפה",
            "שדרות הנשיא 77 חיפה",
            "דרך מנחם בגין 12 נתניה",
            "דרך ז'בוטינסקי 44 נתניה",
            "דרך יפו 56 ראשון לציון",
            "דרך הרצל 14 ראשון לציון",
            "דרך העצמאות 22 רחובות",
            "דרך ירושלים 88 רחובות",
            "דרך בן גוריון 7 אשדוד",
            "דרך העצמאות 19 אשדוד",
            "שדרות ירושלים 15 בני ברק",
            "רחוב רבי עקיבא 28 בני ברק",
            "רחוב הרצל 66 פתח תקווה",
            "רחוב המסגר 9 פתח תקווה",
            "רחוב דגניה 12 כפר סבא",
            "רחוב המלאכה 44 כפר סבא",
            "רחוב הרצל 7 רמלה",
            "רחוב ויצמן 33 רמלה",
            "שדרות הרצל 11 אילת",
            "רחוב הירדן 22 אילת",
            "רחוב דוד המלך 18 אשקלון",
            "רחוב העצמאות 41 אשקלון",
            "רחוב הגליל 12 קריית גת",
            "רחוב הנרקיס 44 קריית גת",
            "רחוב הרצל 5 מודיעין",
            "רחוב התמר 22 מודיעין",
            "רחוב יגאל אלון 18 רמת השרון",
            "רחוב סוקולוב 7 רמת השרון",
            "רחוב אבן גבירול 19 חולון",
            "רחוב לוי אשכול 32 חולון"
        };

        string[] customerFullNames = {
            "David Cohen",
            "Moshe Levi",
            "Yossi Mizrahi",
            "Avi Kaplan",
            "Noam Ben-David",
            "Yaakov Peretz",
            "Eli Shapira",
            "Omer Katz",
            "Itamar Weiss",
            "Daniel Bar",
            "Yonatan Azulay",
            "Ariel Mor",
            "Nadav Shalom"
            , "Asaf Regev",
            "Boaz Nir",
            "Shai Dror",
            "Tomer Giladi",
            "Amir Dahan",
            "Eitan Rosen",
            "Ronen Halevi",
             "Gil Ben-Ami",
            "Tal Oren",
            "Yair Hadad",
            "Harel Romano",
            "Oren Tal",
             "Barak Almog",
            "Lior Peled",
            "Ziv Avraham",
            "Gal Sharon",
            "Nir Ravid",
            "Oded Harel",
            "Rami Goldstein",
            "Erez Shani",
            "Yoav Segal",
            "Itai Barkai",
            "Shlomi Dayan",
            "Guy Arbel",
            "Yinon Aloni",
            "Elad Pardo",
            "Amit Refael",
            "Reuven Azulay",
            "Meir Cohen",
            "Ariel Zaken",
            "Roi Meir",
            "Ofer Tzur",
            "Yehuda Koren",
            "Tzachi Malka",
            "Eyal Ben-Shushan",
            "Ofir Levi",
            "Ran Amar"};

        for (int i = 0; i < 50; i++)
        {
            Order order = new()
            {
                Id = 0,
                OrderType = (OrderType)s_random.Next(0, 3),
                OrderNote = " ",
                CustomerAddress = customerAddresses[i],
                Latitude = s_random.NextDouble() * 90,
                Longitude = s_random.NextDouble() * 180,
                CustomerFullName = customerFullNames[i],
                OrderDate = s_dalConfig!.Clock.AddDays(-s_random.Next(0, 500)),
                OrderProperties = ""
            };
            s_dalOrder!.Create(order);
        }
    }
    private static void createDelivery()
    {
        List<Order> orders = s_dalOrder!.ReadAll();
        List<Courier> couriers = s_dalCourier!.ReadAll();
        for (int i = 0; i < 30; i++)
        {
            Order order = orders[s_random.Next(orders.Count)];
            Courier courier = couriers[s_random.Next(couriers.Count)];
            
            Delivery delivery = new()
            {
                Id = 0,
                OrderId = order.Id,
                CourierId = courier.Id,
                DeliveryType = (DeliveryType)s_random.Next(0, 4),
                DeliveryStartTime = s_dalConfig!.Clock.AddDays(-s_random.Next(0, 500)),
                ActualDistance = null,
                DeliveryTermintionType = (DeliveryTermintionType)s_random.Next(0, 5),
                DeliveryEndTime = null
            };
            s_dalDelivery!.Create(delivery);    
        }
    }
    public static void Do(IConfig? dalConfig, ICourier? dalCourier, IDelivery? dalDelivery,IOrder? dalOrder)
    {
        s_dalCourier = dalCourier?? throw new NullReferenceException("dalCourier is null, DAL object can not be null!");
        s_dalDelivery = dalDelivery ?? throw new NullReferenceException("dalDelivery is null, DAL object can not be null!");
        s_dalOrder = dalOrder ?? throw new NullReferenceException("dalOrder is null, DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("dalConfig is null, DAL object can not be null!");
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalCourier.DeleteAll(); //stage 1
        s_dalDelivery.DeleteAll(); //stage 1
        s_dalOrder.DeleteAll(); //stage 1
        Console.WriteLine("Initializing Couriers list ...");
        createCouriers();
        Console.WriteLine("Initializing Orders list ...");
        createOrders();
        Console.WriteLine("Initializing Delivery list ...");
        createDelivery();
    }
}

