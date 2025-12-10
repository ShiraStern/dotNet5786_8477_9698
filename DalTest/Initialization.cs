namespace Dal;
using DalApi;
using DO;
using System.Numerics;

public static class Initialization
{
    private static IDal? s_dal; //stage 2
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
            while (s_dal!.Courier.Read(id) is not null);
            Courier courier = new()
            {
                Id = id,
                FullName = name,
                Phone = $"05{s_random.Next(0, 9)}-{s_random.Next(1000000, 9999999)}",
                Email = $"{name.Replace(" ", "")}@gmail.com",
                Password = $"{name.Length % 97}{s_random.Next(1000, 9999)}",
                Active = s_random.Next(0, 2) == 1,
                MaxDistance = s_random.Next(40, (int)(s_dal!.Config.MaxRange ?? 297)),
                DeliveryType = (DeliveryType)s_random.Next(0, 4),
                EmploymentStartDate = DateTime.Now.AddYears(-s_random.Next(0, 10))
            };
            s_dal!.Courier.Create(courier);
        }
    }
    private static void createOrders()
    {
        string[] Longitudes =
            {
                "32.062309",
            "32.0642206",
            "32.0678237",
            "32.0762671",
            "32.0940739",
            "32.0892178",
            "31.9656429",
            "31.9726573",
            "32.0878024",
            "32.0815921",
            "31.7649162",
            "31.7666903",
            "31.7809283",
            "31.7754036",
            "32.0726935",
            "32.0829383",
            "32.1871737",
            "32.1897096",
            "32.8074696",
            "32.8000289",
            "31.2382014",
            "31.2649532",
            "32.8074784",
            "32.811325",
            "32.274473",
            "32.324406",
            "31.9625774",
            "31.971035",
            "31.9059957",
            "31.8906061",
            "31.8028711",
            "31.7915357",
            "32.0856982",
            "32.0847936",
            "32.079201",
            "32.091173",
            "32.189569",
            "34.908064",
            "34.930419",
            "31.932824",
            "29.56064",
            "29.55403",
            "31.666355",
            "31.67083",
            "31.609458",
            "31.603389",
            "31.890632",
            "31.868725",
            "32.137817",
            "32.138883",

        };
        // קו רוחב
        string[] Latitudes =
        {
               "34.770001",
            "34.7747952",
            "34.7710218",
            "34.7679687",
            "34.8187769",
            "34.8040052",
            "34.7902183",
            "34.8049313",
            "34.8850274",
            "34.8768377",
            "35.2256463",
            "35.2235846",
            "35.219612",
            "35.2174487",
            "34.7816096",
            "34.7738057",
            "34.876581",
            "34.8546819",
            "34.9923154",
            "35.0169178",
            "34.7833357",
            "34.7972697",
            "34.9984731",
            "34.984288",
            "34.844801",
            "34.849461",
            "34.8314144",
            "34.8055952",
            "34.7954218",
            "34.8105658",
            "34.652159",
            "34.6599768",
            "34.8301849",
            "34.8302302",
            "34.876779",
            "34.877073",
            "34.912614",
            "32.190369",
            "31.932782",
            "34.931751",
            "34.957643",
            "34.952934",
            "34.580766",
            "34.577909",
            "34.76793",
            "34.766723",
            "35.003923",
            "35.006932",
            "34.84661",
            "34.821731",
        };
        
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
            "Ran Amar"
        };
        
        
        for (int i = 0; i < 50; i++)
        {
            //int id = 0;
            //OrderType orderType = (OrderType)s_random.Next(0, 3);
            //string customerAddress = customerAddresses[i];
            //double l1=s_random.NextDouble() * 90;
            //double l2 = s_random.NextDouble() * 180;
            //string customerFullName = customerFullNames[i];
            //string customerPhone=  "05" + Random.Shared.Next(0, 10) + Random.Shared.Next(1000000, 9999999);
            //DateTime orderDate= DateTime.Now.AddDays(-s_random.Next(0, 600));


            Order order = new()
            {
                Id = 0,
                OrderType = (OrderType)s_random.Next(0, 3),
                OrderNote = " ",
                CustomerAddress = customerAddresses[i],
                Latitude = 29 + s_random.NextDouble() * 4,   // Latitude בין 29 ל-33
                Longitude = 34 + s_random.NextDouble() * 2,  // Longitude בין 34 ל-36
                CustomerFullName = customerFullNames[i],
                CustomerPhone = "05" + Random.Shared.Next(0, 10) + Random.Shared.Next(1000000, 9999999),
                OrderDate = DateTime.Now.AddDays(-s_random.Next(0, 600)),
                OrderProperties = ""
            };
            s_dal!.Order.Create(order);
        }
    }
    private static void createDelivery()
    {
        //List<Order> orders = s_dal!.Order.ReadAll()   ;
        //List<Courier> couriers = s_dal!.Courier.ReadAll();
        var orders = s_dal!.Order.ReadAll().ToList();
        var couriers = s_dal!.Courier.ReadAll().ToList();

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
                DeliveryStartTime = DateTime.Now.AddDays(-s_random.Next(0, 500)),
                ActualDistance = null,
                DeliveryTermintionType = (DeliveryTermintionType)s_random.Next(0, 5),
                DeliveryEndTime = null
            };

            s_dal!.Delivery.Create(delivery);
        }


    }
    //public static void Do(IDal? dal) 
    public static void Do() // stage 4
    {
        // s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        Console.WriteLine("Reset Configuration values and List values...");//stage 2

        s_dal.ResetDB();//stage 2
        createOrders();
        createCouriers();
        createDelivery();
       

    }
}

