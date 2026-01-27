namespace DalTest;
using DalApi;
using DO;
using System.Diagnostics;
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
                EmploymentStartDate = s_dal.Config.Clock.AddYears(-s_random.Next(0, 10))
            };
            s_dal!.Courier.Create(courier);
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
            "Ran Amar"
        };
        
        
        for (int i = 0; i < 50; i++)
        {
            Order order = new()
            {
                Id = 0,
                OrderType = (OrderType)s_random.Next(Enum.GetValues<OrderType>().Length),
                OrderNote = " ",
                CustomerAddress = customerAddresses[i],
                Latitude = 0,
                Longitude = 0,
                CustomerFullName = customerFullNames[i],
                CustomerPhone = "05" + Random.Shared.Next(0, 10) + Random.Shared.Next(1000000, 9999999),
                OrderDate = s_dal.Config.Clock.AddDays(-1*s_random.Next(30)).AddHours(-s_random.Next(-23, 0)).AddMinutes(-s_random.Next(-59, 0)),
                OrderProperties = (OrderProperties)s_random.Next(Enum.GetValues<OrderProperties>().Length)
            };
            s_dal!.Order.Create(order);
        }
    }
    
    
    
    private static void createDelivery()
    {
        var orders = s_dal!.Order.ReadAll().ToList();
        var couriers = s_dal!.Courier.ReadAll().ToList();

        for (int i = 0; i < 30; i++)
        {
            Order order = orders[i];
            Courier courier = couriers[s_random.Next(couriers.Count)];

            DateTime startDate;
            DateTime? endDate;
            DO.DeliveryType deliveryType = courier.DeliveryType;

            DO.DeliveryTermintionType terminetionType = 
                (DeliveryTermintionType)s_random.Next(Enum.GetValues<DO.DeliveryTermintionType>().Length);



             startDate = order.OrderDate.AddDays(s_random.Next(4)).AddHours(s_random.Next(24)).AddMinutes(s_random.Next(60));
            if (startDate > s_dal.Config.Clock)
                terminetionType = DO.DeliveryTermintionType.None;


            switch (terminetionType)
            {
                case DeliveryTermintionType.None:
                    endDate = null;
                    break;
                case DeliveryTermintionType.Cancelled:
                    endDate = startDate;
                    break;
                default:
                    endDate = startDate.AddDays(s_random.Next(0, (int)s_dal.Config.MaxDeliveryDuration.TotalDays + 10)).
                       AddHours(s_random.Next(24)).AddMinutes(s_random.Next(60));
                break;
            }

            Delivery delivery = new()
            {
                Id = 0,
                OrderId = order.Id,
                CourierId = courier.Id,
                DeliveryType= deliveryType,
                DeliveryStartTime = startDate,
                ActualDistance = 0,
                DeliveryTermintionType =terminetionType ,
                DeliveryEndTime = endDate
            };

            s_dal!.Delivery.Create(delivery);
        }
    }
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

