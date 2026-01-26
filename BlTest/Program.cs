// See https://aka.ms/new-console-template for more information
using BO;
using DO;
using Helpers;


internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



    private static void GetDeliveriesPerCourier()
    {
        Console.WriteLine("Please enter the ID of applicant and courier ID");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        s_bl.Order.GetDeliveriesPerCourier(applicantID, courierID);
    }//
    private static void GetClosedDeliveriesPerCourier()
    {
        Console.WriteLine("Please enter the ID of applicant and courier ID");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        s_bl.Order.GetClosedDeliveriesPerCourier(applicantID, courierID);
    }//

    private static void EndOrderHandle()
    {
        Console.WriteLine("Please enter the ID of applicant, courier, order and delivery");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        int deliveryID = int.Parse(Console.ReadLine()!);
        //s_bl.Order.EndOrderHandle(applicantID, courierID, orderID, deliveryID);
    }//
    private static void AddOrder()
    {
        Console.WriteLine("Please enter the ID of applicant and values of order to add");
        int applicantID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        // create BO.Order
        Console.WriteLine();
        int id = int.Parse(Console.ReadLine()!);
        BO.OrderType orderType = (BO.OrderType)int.Parse(Console.ReadLine()!);
        string VerbalDescription = Console.ReadLine()!;
        string FullAddressOfTheOrder = Console.ReadLine()!;
        double AirDistance = double.Parse(Console.ReadLine()!);
        string FullNameOfTheInviter = Console.ReadLine()!;
        string OrderersPhoneNumber = Console.ReadLine()!;
        BO.OrderProperties orderProperties = (BO.OrderProperties)int.Parse(Console.ReadLine()!);
        DateTime OrderOpeningTime = DateTime.Parse(Console.ReadLine()!);
        DateTime EstimatedDeliveryTime = DateTime.Parse(Console.ReadLine()!);
        DateTime MaximumDeliveryTime = DateTime.Parse(Console.ReadLine()!);
        BO.OrderStatus OrderStatus = (BO.OrderStatus)int.Parse(Console.ReadLine()!);
        BO.ScheduleStatus ScheduleStatus = (ScheduleStatus)int.Parse(Console.ReadLine()!);
        int TimeLeftToCompleteOrder = int.Parse(Console.ReadLine()!);
        BO.Order order = new BO.Order
        {
            ID = id,
            OrderType = orderType,
            VerbalDescription = VerbalDescription,
            FullAddressOfTheOrder = FullAddressOfTheOrder,
            AirDistance = AirDistance,
            FullNameOfTheInviter = FullNameOfTheInviter,
            OrderersPhoneNumber = OrderersPhoneNumber,
            OrderProperties = orderProperties,
            OrderOpenDate = OrderOpeningTime,
            EstimatedDeliveryDate = EstimatedDeliveryTime,
            MaximumDeliveryDate = MaximumDeliveryTime,
            OrderStatus = OrderStatus,
            ScheduleStatus = ScheduleStatus,
            TimeLeftToCompleteOrder = TimeSpan.FromDays(TimeLeftToCompleteOrder)
        };
        //  להוסיף חישוב של קאורדינטות
        s_bl.Order.UpdateDetails(applicantID, order);
    }//
    private static void DeleteOrder()
    {
        Console.WriteLine("Please enter the ID of applicant and order ID");
        int applicantID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        s_bl.Order.Delete(applicantID, orderID);
    }//
    
    private static void CancelOrder()
    {
        Console.WriteLine("Please enter the ID of applicant and order ID");
        int applicantID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        s_bl.Order.CancelOrder(applicantID, orderID);
    }//
    private static void UpdateOrderDetails()
    {
        Console.WriteLine("Please enter the ID of applicant and ID of order to update");
        int applicantID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        // create BO.Order
        Console.WriteLine();
        int id= int.Parse(Console.ReadLine()!);
        BO.OrderType orderType= (BO.OrderType)int.Parse(Console.ReadLine()!);
        string VerbalDescription = Console.ReadLine()!;
        string FullAddressOfTheOrder= Console.ReadLine()!;
        double AirDistance = double.Parse(Console.ReadLine()!);
        string FullNameOfTheInviter= Console.ReadLine()!;
        string OrderersPhoneNumber = Console.ReadLine()!;
        BO.OrderProperties orderProperties = (BO.OrderProperties)int.Parse(Console.ReadLine()!);
        DateTime OrderOpeningTime= DateTime.Parse(Console.ReadLine()!);
        DateTime EstimatedDeliveryTime= DateTime.Parse(Console.ReadLine()!);
        DateTime MaximumDeliveryTime= DateTime.Parse(Console.ReadLine()!);
        BO.OrderStatus OrderStatus= (BO.OrderStatus)int.Parse( Console.ReadLine()!);    
        BO.ScheduleStatus ScheduleStatus= (ScheduleStatus)int.Parse( Console.ReadLine()!);
        int TimeLeftToCompleteOrder= int.Parse(Console.ReadLine()!);
        BO.Order order = new BO.Order
        {
            ID = id,
            OrderType = orderType,
            VerbalDescription = VerbalDescription,
            FullAddressOfTheOrder = FullAddressOfTheOrder,
            AirDistance = AirDistance,
            FullNameOfTheInviter = FullNameOfTheInviter,
            OrderersPhoneNumber = OrderersPhoneNumber,
            OrderProperties = orderProperties,
            OrderOpenDate = OrderOpeningTime,
            EstimatedDeliveryDate = EstimatedDeliveryTime,
            MaximumDeliveryDate = MaximumDeliveryTime,
            OrderStatus = OrderStatus,
            ScheduleStatus = ScheduleStatus,
            TimeLeftToCompleteOrder = TimeSpan.FromDays(TimeLeftToCompleteOrder)
        };
        //  להוסיף חישוב של קאורדינטות
        s_bl.Order.UpdateDetails(applicantID, order);
    }//

    private static void GetOrderDetails()
    {
        Console.WriteLine("Please enter the ID of applicant and order ID");
        int applicantID = int.Parse(Console.ReadLine()!);
        int orderID = int.Parse(Console.ReadLine()!);
        s_bl.Order.GetDetails(applicantID, orderID);
    }//
    private static void GetOrderList()
    {
        Console.WriteLine("Please enter the ID of applicant");
        int applicantID = int.Parse(Console.ReadLine()!);
        s_bl.Order.GetOrderList(applicantID);
    }//
    private static void GetOrdersStatusCounts()
    {
        Console.WriteLine("Please enter the ID of applicant");
        int applicantID = int.Parse(Console.ReadLine()!);
        s_bl.Order.GetOrdersStatusCounts(applicantID);
    }//

    private static void UpdateCourierDetails()
    {
        //    Console.WriteLine("Please enter the ID of applicant and ID of specipic courier");
        //    int applicantID = int.Parse(Console.ReadLine()!);
        //    int courierID = int.Parse(Console.ReadLine()!);
        //    Console.WriteLine("Please enter the new details for the courier");
        //    Console.WriteLine("Enter the new name:");
        //    string? name = Console.ReadLine();
        //    Console.WriteLine("Enter the new phone number:");
        //    string? phone = Console.ReadLine();
        //    Console.WriteLine("Enter the new Email:");
        //    string? Email = Console.ReadLine();
        //    Console.WriteLine("Enter the new Password:");
        //    string? Password = Console.ReadLine();
        //    Console.WriteLine("Enter new value, Is coureir active?:");
        //    bool active = bool.Parse(Console.ReadLine()!);
        //    Console.WriteLine("Enter new value for maximum distance:");
        //    double? maxDistance = double.Parse(Console.ReadLine()!);
        //    Console.WriteLine(@"Please enter a new value for delivery type
        //1- walking
        //2- Bicycle,
        //3- Motorcycle,
        //4- Car");
        //    BO.DeliveryType DeliveryType = (BO.DeliveryType)int.Parse(Console.ReadLine()!);
        //    Console.WriteLine(@"Please enter a new date for start employment:");
        //    DateTime? employmentStartDate = DateTime.Parse(Console.ReadLine()!);
        //    BO.OrderInProgress? orderInProgress = s_bl.
        //    BO.Courier courier = new BO.Courier();
        //    {
        //        ID = courierID ,
        //        FullName = name,
        //        PhoneNember = phone,
        //        Email = Email,
        //        Password = Password,
        //        Active = active,
        //        MaxDistance = maxDistance

        //    }
        //;
        //s_bl.courier.UpdateDetails(applicantID, courier);
        //Console.WriteLine("Courier details updated successfully in test.");
    }
    private static void GetCourierDetails()
    {
        Console.WriteLine("Please enter the ID of applicant and ID of specipic courier");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        BO.Courier? courier = s_bl.Courier.GetDetails(applicantID, courierID);
        Console.WriteLine(courier);
    }//
    private static void ForwardClock()
    {

        Console.WriteLine(@"Please enter a new value for time unit to forward
    1- Minute,
    2- Hour,
    3- Day,
    4- Month,
    5- Year");
        int choice = int.Parse(Console.ReadLine()!);
        switch ((TimeUnit)choice)
        {
            case TimeUnit.Minute:
                s_bl.Admin.ForwardClock(TimeUnit.Minute);
                break;
            case TimeUnit.Hour:
                s_bl.Admin.ForwardClock(TimeUnit.Hour);
                break;
            case TimeUnit.Day:
                s_bl.Admin.ForwardClock(TimeUnit.Day);
                break;
            case TimeUnit.Month:
                s_bl.Admin.ForwardClock(TimeUnit.Month);
                break;
            case TimeUnit.Year:
                s_bl.Admin.ForwardClock(TimeUnit.Year);
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
        Console.WriteLine("Clock forwarded successfully.");

    }//
    private static void SetConfig()
    {
        Console.WriteLine("Enter the new delivery max range:");
        double? maxRange = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new Manager ID");
        int managerID = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new company's address");
        string? companyAddress = Console.ReadLine();
        Console.WriteLine("Enter the new average car's speed");
        double avgCarSpeed = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new average motorcycle's speed");
        double avgMotorcycleSpeed = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new average bicycle's speed");
        double avgBicycleSpeed = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new average Walking speed");
        double avgWalkingSpeed = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Please enter a new value for MaxDeliveryDuration");
        double maxDeliveryDuration = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Please enter a new value for DelayRiskTime");
        double delayRiskTime = double.Parse(Console.ReadLine()!);

        Console.WriteLine("Please enter a new value for InactivityThreshold");
        double inactivityThreshold = double.Parse(Console.ReadLine()!);

        BO.Config config = new BO.Config
        {
            MaxRange = maxRange,
            ManagerID = managerID,
            Clock = DateTime.Now,
            CompanyAddress = companyAddress,
            Latitude = null,
            Longitude = null,
            AvgCarSpeed = avgCarSpeed,
            AvgMotorcycleSpeed = avgMotorcycleSpeed,
            AvgBicycleSpeed = avgBicycleSpeed,
            AvgWalkingSpeed = avgWalkingSpeed,
            MaxDeliveryDuration = TimeSpan.FromDays(maxDeliveryDuration),
            DelayRiskTime = TimeSpan.FromDays(delayRiskTime),
            InactivityThreshold = TimeSpan.FromDays(inactivityThreshold)
        };
        s_bl.Admin.SetConfig(config);
        Console.WriteLine("Configuration updated successfully.");

    }//
    private static void GetCourierList()//
    {
        Console.WriteLine("Please enter the ID of applicant");
        int applicantID = int.Parse(Console.ReadLine()!);
        foreach (var courier in s_bl.Courier.GetCourierList(applicantID, true, BO.FilterCouriersByProperty.IsActive))

        {
            Console.WriteLine(courier.ToString());
        }
    }
    private static void DeleteCourier()
    {
        Console.WriteLine("Please enter the ID of applicant and ID of specipic courier");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        BO.Courier? courier = s_bl.Courier.GetDetails(applicantID, courierID);
        s_bl.Courier.Delete(applicantID, courierID);
    }//
    private static void Login()//
    {
        Console.WriteLine("Please enter the user name ");
        string userName = Console.ReadLine()!;
        Console.WriteLine("Please enter password:");
        string password = Console.ReadLine()!;
        string status = s_bl.Courier.Login(userName, password);
        Console.WriteLine($"{status} have successfully connected!");
    }
    private static void AddCourier()//
    {
        Console.WriteLine("Please enter the ID of applicant and ID of specipic courier");
        int applicantID = int.Parse(Console.ReadLine()!);
        int courierID = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Please enter the new details for the courier");
        Console.WriteLine("Enter the new name:");
        string? name = Console.ReadLine();
        Console.WriteLine("Enter the new phone number:");
        string? phone = Console.ReadLine();
        Console.WriteLine("Enter the new Email:");
        string? Email = Console.ReadLine();
        Console.WriteLine("Enter the new Password:");
        string? Password = Console.ReadLine();
        Console.WriteLine("Enter new value, Is coureir active?:");
        bool active = bool.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter new value for maximum distance:");
        double? maxDistance = double.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Please enter a new value for delivery type
        1- walking
        2- Bicycle,
        3- Motorcycle,
        4- Car");
        BO.DeliveryType DeliveryType = (BO.DeliveryType)int.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Please enter a new date for start employment:");
        DateTime employmentStartDate = DateTime.Parse(Console.ReadLine()!);

        BO.Courier courier = new BO.Courier()
        {
            ID = courierID,
            FullName = name,
            PhoneNember = phone,
            Email = Email,
            Password = Password,
            Active = active,
            MaxDistance = maxDistance,
            DeliveryType = DeliveryType,
            EmploymentStartDate = employmentStartDate
        };
   
        s_bl.Courier.AddCourier(applicantID, courier);
        Console.WriteLine("Courier added successfully");
    }

    private static void AdminMenu()
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {
                Console.WriteLine(@$"Please enter a number to choose an action:
1- Exit
2- Get Clock
3- Forward Clock
4- Reset Data Base
5- Initialize Data Base
6- Get Config
7- Set Config");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((AdminMenuOptions)choice)
            {
                case AdminMenuOptions.Exit:
                    //endProgram
                    continueLoop=false; 
                    break;
                case AdminMenuOptions.GetClock: 
                    s_bl.Admin.GetClock();
                    break;
                 case AdminMenuOptions.ForwardClock:
                    // 
                    ForwardClock();
                    break;
                case AdminMenuOptions.ResetDataBase:
                    s_bl.Admin.ResetDB();
                    break;
                case AdminMenuOptions.InitializeDataBase:
                    s_bl.Admin.InitializeDB();
                    break;
                case AdminMenuOptions.GetConfig:
                    s_bl.Admin.GetConfig();
                    break;
                case AdminMenuOptions.SetConfig:
                   //
                    SetConfig();
                    break;
                default:
                    Console.WriteLine("Incorrect Choice.");
                    break;

            }
        } while (continueLoop);
    }//
    private static void CourierMenu()
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {
                Console.WriteLine(@$"Please enter a number to choose an action:
1- Exit,
2- GetDetails,
3- UpdateDetails,
4- GetCourierList,
5- DeleteCourier,
6- Login,
7- AddCourier");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((CoureirMenuOptions)choice)
            {
                case CoureirMenuOptions.Exit:
                    //endProgram
                    continueLoop = false;
                    break;
                case CoureirMenuOptions.GetDetails:
                    GetCourierDetails();
                    break;
                case CoureirMenuOptions.UpdateDetails:
                    UpdateCourierDetails();
                    break;
                case CoureirMenuOptions.GetCourierList:
                    GetCourierList();
                    break;
                case CoureirMenuOptions.DeleteCourier:
                     DeleteCourier();
                    break;
                case CoureirMenuOptions.Login:
                    Login();
                    break;
                case CoureirMenuOptions.AddCourier:
                    AddCourier();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
        } while (continueLoop);
    }//
    private static void OrderMenu()
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {
                Console.WriteLine(@$"Please enter a number to choose an action:
1-Exit
2-Get Orders Status Counts
3-Get Order List
4-Get Details
5-Update Details
6-Cancel Order
7-Delete Order
8- Add Order
9- End Order Handle
10-Handle Order
11-Get Closed Deliveries Per Courier
12-Get Deliveries Per Courier");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((OrderMenuOptions)choice)
            {
                case OrderMenuOptions.Exit:
                    continueLoop = false;
                    break;
                case OrderMenuOptions.GetOrdersStatusCounts:
                    GetOrdersStatusCounts();
                    break;
                case OrderMenuOptions.GetOrderList:
                    GetOrderList();
                    break;
                case OrderMenuOptions.GetDetails:
                    GetOrderDetails();
                    break;
                case OrderMenuOptions.UpdateDetails:
                    UpdateOrderDetails();
                    break;
                case OrderMenuOptions.CancelOrder:
                    CancelOrder();
                    break;
                case OrderMenuOptions.Delete:
                    DeleteOrder();
                    break;
                case OrderMenuOptions.AddOrder:
                    AddOrder();
                    break;
                case OrderMenuOptions.EndOrderHandle:
                    EndOrderHandle();
                    break;
                case OrderMenuOptions.GetClosedDeliveriesPerCourier:
                    GetClosedDeliveriesPerCourier();
                    break;
                case OrderMenuOptions.GetDeliveriesPerCourier:
                    GetDeliveriesPerCourier();
                    break;


            }

        } while (continueLoop);
    }//
    private static int mainMenuOptions()
    {

        int? choice = null;
        do
        {
            Console.WriteLine(@"Please enter a number to choose an action:
0-Exit
1-CourierMenu
2-OrderMenu
3-AdminMenu");
   
            choice = int.Parse(Console.ReadLine()!);
        } while (choice is null);
        return (int)choice;

    }
    
    static void Main(string[] args)
    {
        try
        {
            
            int choice = mainMenuOptions();
            while (choice != 0)
            {
                switch ((MainMenuOptions)choice)
                {
                    case MainMenuOptions.Exit:
                        choice = 0;
                        break;
                    case MainMenuOptions.CourierMenu:
                        CourierMenu();
                        break;
                    case MainMenuOptions.OrderMenu:
                        OrderMenu();
                        break;
                    case MainMenuOptions.AdminMenu:
                        AdminMenu();
                        break;
                }
                choice = mainMenuOptions();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during data initialization: {ex.Message}");
        }

    }

}
