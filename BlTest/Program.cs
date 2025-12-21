// See https://aka.ms/new-console-template for more information
using BO;
using DO;


internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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
        Console.WriteLine("Clock forwarded successfully.");)

    }
    private static void SetConfig()
    {
        Console.WriteLine("Enter the new delivery max range:");
        double? maxRange = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the new Manager ID");
        int managerID=int.Parse(Console.ReadLine()!);
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
            Clock= DateTime.Now,
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

    }


    private static void AdminMenu(
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
    }
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
                    break;
                case CoureirMenuOptions.GetDetails:
                    s_bl.courier.GetDetails();
                    break;
                case CoureirMenuOptions.UpdateDetails:
                    viewDelivery();
                    break;
                case CoureirMenuOptions.GetCourierList:
                    viewAllDeliveries();
                    break;
                case CoureirMenuOptions.DeleteCourier:
                    updateDelivery();
                    break;
                case CoureirMenuOptions.Login:
                    deleteDelivery();
                    break;
                case CoureirMenuOptions.AddCourier:
                    deleteAllDeliveries();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
        } while (continueLoop);
    }
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
2-AddDelivery
3-ViewDeliveries
4-viewAllDeliveries
5-UpdateDelivery
6-DeleteDelivery
7-DeleteAllDeliveries");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((DeliveryMenu)choice)
            {
                case DeliveryMenu.Exit:
                    //endProgram
                    break;
                case DeliveryMenu.AddDelivery:
                    addDelivery();
                    break;
                case DeliveryMenu.ViewDelivery:
                    viewDelivery();
                    break;
                case DeliveryMenu.ViewAllDeliveries:
                    viewAllDeliveries();
                    break;
                case DeliveryMenu.UpdateDelivery:
                    updateDelivery();
                    break;
                case DeliveryMenu.DeleteDelivery:
                    deleteDelivery();
                    break;
                case DeliveryMenu.DeleteAllDeliveries:
                    deleteAllDeliveries();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
        } while (continueLoop);
    }

    static void Main(string[] args)
    {

    }
}