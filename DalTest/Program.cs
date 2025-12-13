namespace DalTest;
using DalApi;
using DO;
using Dal;
using System.Threading.Channels;




internal class Program
{
    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXML(); //stage 3
    static readonly IDal s_dal =Factory.Get; //stage 4  

    //private static ICourier? s_dalCourier = new CourierImplementation(); //stage 1
    //private static IDelivery? s_dalDelivery = new DeliveryImplementation(); //stage 1
    //private static IOrder? s_dalOrder = new OrederImplementation(); //stage 1
    //private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    //---------------------------------------------------------------------------------------------------------------------------------------

    // courier mnue functions
    private static void addCourier() 
    {
        Console.WriteLine("Please enter the following details:");

        Console.Write("ID number: ");
        int id = int.Parse(Console.ReadLine() ?? "0");

        Courier? tempCourier = s_dal.Courier.Read(id);

        if (tempCourier != null)
        {
            throw new DalAlreadyExistsException($"Courier with ID {id} already exists");
        }
        

        Console.Write("Full name: ");
        string fullName = Console.ReadLine() ?? "";

        Console.Write("Phone number: ");
        string phone = Console.ReadLine() ?? "";

        Console.Write("Email address: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        Console.Write("Maximum delivery distance (optional, press Enter to skip): ");
        string? distanceInput = Console.ReadLine();
        double? maxDistance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);

        Console.Write("Delivery type (enter number: 0-Car, 1-Motorcycle, 2-Drone, 3-Walking): ");
        DeliveryType deliveryType = (DeliveryType)int.Parse(Console.ReadLine() ?? "0");

        DateTime employmentStartDate;

        while (true)
        {
            Console.Write("Employment start date (YYYY-MM-DD): ");
            string? input = Console.ReadLine();

            try
            {
                employmentStartDate = DateTime.Parse(input!);

                int month = employmentStartDate.Month;
                int day = employmentStartDate.Day;

                if (month < 1 || month > 12 || day < 1 || day > 31)
                {
                    Console.WriteLine("Invalid date values. Please try again.");
                    continue;
                }

                break; // ✔ תאריך תקין
            }
            catch
            {
                Console.WriteLine("Invalid date format. Please enter again.");
            }
        }


        Courier courier = new Courier()
        {
            Id = id,
            FullName = fullName,
            Phone = phone,
            Email = email,
            Password = password,
            Active = true,
            MaxDistance = maxDistance,
            DeliveryType = deliveryType,
            EmploymentStartDate = employmentStartDate
        };
        s_dal.Courier.Create(courier);
        Console.WriteLine("Courier created successfully!");

    }
    private static void viewCourier()
    {
        Console.WriteLine("Please enter  ID number of courier you wish to display.");
        int? id= int.Parse(Console.ReadLine());
        if (id == null)
            throw new Exception(@"invalid ID");
        Console.WriteLine(s_dal.Courier.Read((int)id)); 

    }
    private static void viewAllCouriers()
    {
        foreach (var item in s_dal.Courier!.ReadAll())
        {
            Console.WriteLine(item.ToString());
        }
    }
    private static void updateCourier()
    {
        Console.Write("Enter ID to update: ");
        int id = int.Parse(Console.ReadLine() ?? "0");

        Courier? existing = s_dal.Courier.Read(id);
        if (existing == null)
        {
            Console.WriteLine("Courier not found.");
            return;
        }

        Console.WriteLine("Enter new values (press Enter to keep current)");

        // קריאה לקלט ובדיקה. אם הקלט ריק, שומרים את הערך הקיים.
        // שדות המחרוזת:
        Console.Write($"Full name ({existing.FullName}): ");
        string? fullName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(fullName)) fullName = existing.FullName; // אם ריק, שומרים את הקיים

        Console.Write($"Phone ({existing.Phone}): ");
        string? phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone)) phone = existing.Phone; // אם ריק, שומרים את הקיים

        Console.Write($"Email ({existing.Email}): ");
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email)) email = existing.Email; // אם ריק, שומרים את הקיים

        Console.Write($"Password ({existing.Password}): ");
        string? password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password)) password = existing.Password; // אם ריק, שומרים את הקיים

        // שדות בוליאניים:
        Console.Write($"Is active ({existing.Active}): ");
        string? activeInput = Console.ReadLine();
        bool active = string.IsNullOrWhiteSpace(activeInput) ? existing.Active : bool.Parse(activeInput);

        // שדות דאבל אופציונליים:
        Console.Write($"Max distance ({existing.MaxDistance}): ");
        string? mdInput = Console.ReadLine();
        double? maxDistance = string.IsNullOrWhiteSpace(mdInput) ? existing.MaxDistance : double.Parse(mdInput);

        // שדות Enum:
        Console.Write("Delivery type (0-Car,1-Motorcycle,2-Drone,3-Walking). Current " +
                      $"({(int)existing.DeliveryType}): ");
        string? dtInput = Console.ReadLine();
        DeliveryType deliveryType =
            string.IsNullOrWhiteSpace(dtInput) ? existing.DeliveryType : (DeliveryType)int.Parse(dtInput);

        // שדות תאריך:
        Console.Write($"Employment start date ({existing.EmploymentStartDate:yyyy-MM-dd}): ");
        string? dateInput = Console.ReadLine();
        DateTime employmentDate =
            string.IsNullOrWhiteSpace(dateInput) ? existing.EmploymentStartDate : DateTime.Parse(dateInput);

        // שלב 2: יצירת אובייקט חדש באמצעות 'with' עם הערכים המוכנים (התיקון העיקרי).
        Courier updated = existing with
        {
            // שימוש במשתנים המקומיים המכילים את הערכים הנכונים (החדשים או הישנים),
            // במקום קריאה חוזרת ל-Console.ReadLine().
            FullName = fullName,
            Phone = phone,
            Email = email,
            Password = password,

            Active = active,
            MaxDistance = maxDistance,
            DeliveryType = deliveryType,
            EmploymentStartDate = employmentDate
        };

        s_dal.Courier.Update(updated);

        Console.WriteLine("Courier updated successfully!");
    }

    private static void deleteCourier()
    {
        Console.WriteLine("Please enter ID number of courier you wish to delete."); 
        int? id = int.Parse(Console.ReadLine());//לבדוק למה הוא מסמן כאן ירוק
        if (id == null)
            throw new Exception(@"invalid courier ID");
        if (s_dal.Delivery!.ReadAll(d => d.CourierId == id).Any())
            throw new DalDoesNotExistException($"Cannot delete courier with ID {id} because there are deliveries associated with it.");
        s_dal.Courier.Delete((int)id); 
        Console.WriteLine($"Courier with ID {id} deleted successfully.");
    }
    private static void deleteAllCouriers() 
    { 
        s_dal.Courier.DeleteAll();
        Console.WriteLine("All couriers have been successfully deleted!");
    }

    // order mnue functions

    private static void addOrder()
    {
        Console.WriteLine(" Adding New Order ");

        // משתנים לשמירת הקלט הבטוח
        int id;
        string? input;

        // קליטת ID (0 לבחירה אוטומטית) - שימוש ב-TryParse
        do
        {
            Console.Write("ID number (0 for auto-generate, or specific ID): ");
            input = Console.ReadLine();
            if (int.TryParse(input, out id) && id >= 0)
            {
                break; // יציאה מהלולאה אם הקלט הוא מספר שלם אי-שלילי
            }
            Console.WriteLine(" Error: ID must be a valid non-negative number (0 or positive). Please try again.");
        } while (true);

        // בדיקת ID קיים (אם הוכנס ID ספציפי)
        if (id != 0)
        {
            try
            {
                s_dal.Order!.Read(id);
                throw new DalAlreadyExistsException($"Order with ID {id} already exists.");
            }
            catch (DalDoesNotExistException)
            {
                // ID לא קיים, אפשר להמשיך
            }
        }

        // קליטת פרטי לקוח וסוג הזמנה
        Console.Write("Customer full name: ");
        string customerFullName = Console.ReadLine() ?? "";

        Console.Write("Customer phone number: ");
        string customerPhone = Console.ReadLine() ?? "";

        Console.Write("Customer address: ");
        string customerAddress = Console.ReadLine() ?? "";

        //  קליטת Order Type - שימוש ב-TryParse ובדיקת Enum
        OrderType orderType;
        do
        {
            Console.Write("Order type (enter number: 0-Small, 1-Medium, 2-Large): ");
            string? typeInput = Console.ReadLine();

            if (int.TryParse(typeInput, out int typeValue) && Enum.IsDefined(typeof(OrderType), typeValue))
            {
                orderType = (OrderType)typeValue;
                break; // יציאה מהלולאה אם הקלט חוקי
            }
            Console.WriteLine(" Error: Invalid order type. Please enter 0, 1, or 2.");
        } while (true);

        Console.Write("Order note (verbal description): ");
        string orderNote = Console.ReadLine() ?? "";

        Console.Write("Order additional properties (optional, Enter to skip): ");
        string? orderProperties = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(orderProperties)) orderProperties = null;


        //  קליטת מיקום (Latitude ו-Longitude) - שימוש ב-TryParse
        double latitude;
        double longitude;

        Console.WriteLine("Enter Delivery Destination Location:");

        do
        {
            Console.Write("Latitude (double): ");
            if (double.TryParse(Console.ReadLine(), out latitude)) break;
            Console.WriteLine(" Error: Invalid latitude format. Please enter a number.");
        } while (true);

        do
        {
            Console.Write("Longitude (double): ");
            if (double.TryParse(Console.ReadLine(), out longitude)) break;
            Console.WriteLine(" Error: Invalid longitude format. Please enter a number.");
        } while (true);


        //  קליטת תאריך ההזמנה - שימוש ב-TryParse
        DateTime orderDate;
        Console.Write($"Order date (YYYY-MM-DD, or Enter for today: {DateTime.Now:yyyy-MM-dd}): ");
        string? dateInput = Console.ReadLine();

        // בדיקה האם הקלט ריק או שהפורמט אינו חוקי
        if (string.IsNullOrWhiteSpace(dateInput) || !DateTime.TryParse(dateInput, out orderDate))
        {
            orderDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(dateInput))
            {
                Console.WriteLine("Warning: Invalid date format provided. Setting order date to today.");
            }
        }

        // יצירת אובייקט Order חדש 
        DO.Order newOrder = new DO.Order
        (
            Id: id,
            OrderType: orderType,
            OrderNote: orderNote,
            CustomerAddress: customerAddress,
            Latitude: latitude,
            Longitude: longitude,
            CustomerFullName: customerFullName,
            CustomerPhone: customerPhone,
            OrderDate: orderDate,
            OrderProperties: orderProperties
        );

        // 6. קריאה לפונקציית Create 
        try
        {
            s_dal.Order!.Create(newOrder);
            Console.WriteLine(" Order added successfully!");
        }
        catch (Exception ex)
        {
            // יש לטפל בשגיאות שעלולות לקרות ב-DAL, כמו חריגה בגלל ערכים לא חוקיים
            Console.WriteLine($"An error occurred while creating the order: {ex.Message}");
        }
        // Function to add a new Order object to the Data Base.
        try
        {
            // Basic Data Collection from the user
            Console.Write("Enter Customer Name: ");
            string customerName = Console.ReadLine() ?? "";

            Console.Write("Enter Customer Phone: ");
            string customerPhoneInput = Console.ReadLine() ?? "";
            Console.Write("Enter Customer Address: ");
            string address = Console.ReadLine() ?? "";

            Console.Write("Enter Order Note (optional): ");
            string note = Console.ReadLine() ?? "";

            // 2. Handling the OrderType Enum (e.g., Large, Medium, Small)
            Console.WriteLine("Enter Order Type (Large, Medium, Small). Default is Medium if invalid input is provided: ");
            string typeInput = Console.ReadLine() ?? "Medium";
            DO.OrderType type;

            // Attempt to parse the user's input string into the DO.OrderType Enum.
            // The 'true' argument makes the comparison case-insensitive.
            if (!Enum.TryParse(typeInput, true, out type))
            {
                // Fallback to the default value if parsing fails
                type = DO.OrderType.Medium;
                Console.WriteLine($"Invalid Order Type entered. Defaulting to: {type}");
            }

            // 3. Creating the New Order Object
            // The ID is set to 0, assuming the DAL/DataSource will assign the actual unique ID.
            // Latitude and Longitude are set to 0 for simplicity, based on the provided record structure.
            DO.Order orderToCreate = new DO.Order(
     Id: 0,
     OrderType: type,
     OrderNote: note,
     CustomerAddress: address,
     Latitude: 0,
     Longitude: 0,
     CustomerFullName: customerName,
     CustomerPhone: customerPhone,
     OrderDate: DateTime.Now,
     OrderProperties: null
 );

            // 4. Adding the object to the Data Source (e.g., a static list or DAL method)

            // Assuming there is a static list named 'Orders' in the scope:
            // Orders.Add(newOrder); 

            // If integrating with the DAL interface (IBl or IDal):
            // s_dal.Order.Create(newOrder); 

            Console.WriteLine("Order added successfully!");
        }
        catch (Exception ex)
        {
            // Display an error message if any part of the process fails (e.g., IO error).
            Console.WriteLine($"ERROR: Failed to add order. {ex.Message}");
        }
    }
    private static void viewOrder()
    {
        Console.WriteLine(" Displaying Order Details");

        int id;
        string? input;

        // קליטת ID בטוחה (שימוש ב-TryParse)
        do
        {
            Console.Write("Please enter the ID number of the order you wish to display: ");
            input = Console.ReadLine();

            // בדיקה שהקלט הוא מספר חיובי תקין
            if (int.TryParse(input, out id) && id > 0)
            {
                break; // יציאה מהלולאה אם הקלט תקין
            }
            Console.WriteLine(" Error: Order ID must be a positive number. Please try again.");
        } while (true);

        // 2. קריאה ל-DAL וטיפול בשגיאות
        try
        {
            // קריאת האובייקט מה-DAL (השיטה Read צריכה לקבל int)
            DO.Order? orderToDisplay = s_dal.Order.Read(id);

            if (orderToDisplay == null)
            {
                throw new DalDoesNotExistException($"Order with ID {id} does not exist");
            }

            Console.WriteLine(orderToDisplay);

            // הדפסת האובייקט (בהנחה של-DO.Order יש הטמעת ToString טובה)
            Console.WriteLine(orderToDisplay);

            Console.WriteLine(" Order details displayed successfully.");
        }
        catch (DalDoesNotExistException ex)
        {
            // טיפול במקרה שה-ID לא נמצא במערכת
            Console.WriteLine($" Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // טיפול בשאר השגיאות
            Console.WriteLine($" An unexpected error occurred: {ex.Message}");
        }
    }
    private static void viewAllOrders()
    {
        Console.WriteLine(" Displaying All Orders");

        try
        {
            // קריאה ל-DAL לקבלת כל ההזמנות
            IEnumerable<DO.Order> allOrders = s_dal.Order!.ReadAll();

            //  בדיקה אם הרשימה ריקה
            if (!allOrders.Any())
            {
                Console.WriteLine(" The system contains no orders to display.");
                return;
            }

            //  מעבר על הרשימה והדפסה
            foreach (DO.Order item in allOrders)
            {
                Console.WriteLine(item.ToString());
                Console.WriteLine("/n "); // הפרדה ויזואלית
            }

            Console.WriteLine($" Successfully displayed {allOrders.Count()} orders.");
        }
        catch (Exception ex)
        {
            // טיפול בשגיאות שעלולות לקרות במהלך הקריאה מה-DAL
            Console.WriteLine($" An unexpected error occurred while reading orders: {ex.Message}");
        }
    }
    private static void updateOrder()
    {
        
        int id;
        Console.WriteLine("Enter Order ID:");

        // קבלת מזהה (ID) של ההזמנה
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
        }

        // קריאת ההזמנה מהשכבת הגישה לנתונים (DAL)
        DO.Order? order = s_dal.Order.Read(id);
        if (order == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        Console.WriteLine($"\n--- Updating Order ID: {order.Id} ---");

        // עדכון כתובת לקוח
        Console.WriteLine($"Current Customer Address: {order.CustomerAddress}. Enter new address (or press Enter to keep current):");
        string newAddress = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newAddress))
        {
            order = order with { CustomerAddress = newAddress };
        }

        // עדכון שם הלקוח (נניח שקיים שדה כזה)
        Console.WriteLine($"Current Customer Name: {order.CustomerFullName}. Enter new name (or press Enter to keep current):");
        string newName = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            order = order with { CustomerFullName = newName };
        }

        // עדכון תאריך יצירת ההזמנה (OrderDate)
        Console.WriteLine($"Current Order Date: {order.OrderDate}. Enter new date (format: yyyy-MM-dd HH:mm:ss or press Enter to keep current):");
        string newOrderDateInput = Console.ReadLine()!;
        if (DateTime.TryParse(newOrderDateInput, out DateTime newOrderDate))
        {
            order = order with { OrderDate = newOrderDate };
        }

        // עדכון הערות להזמנה (OrderNote)
        Console.WriteLine($"Current Note: {order.OrderNote}. Enter new note (or press Enter to keep current):");
        string newNote = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newNote))
        {
            order = order with { OrderNote = newNote };
        }

        // עדכון מאפיינים נוספים (OrderProperties - שדה אופציונלי/nullable)
        Console.WriteLine($"Current Additional Properties: {order.OrderProperties ?? "(None)"}. Enter new properties (or press Enter to keep current):");
        string newProperties = Console.ReadLine()!;

        // אם הוזן ערך (אפילו ריק כדי לאפס ל-null), מעדכנים
        if (newProperties != null)
        {
            string? propertiesToSet = string.IsNullOrWhiteSpace(newProperties) ? null : newProperties;
            order = order with { OrderProperties = propertiesToSet };
        }


        // קריאה לעדכון במערכת
        s_dal.Order.Update(order);
        Console.WriteLine("Order details updated.");
    }
    private static void deleteOrder()
    {
        Console.WriteLine("Please enter order ID number you wish to deiete.");
        int? id = int.Parse(Console.ReadLine());
        if (id == null)
            throw new Exception(@"invalid order ID");
       s_dal.Order.Delete((int)id);
    }
    private static void deleteAllOrders() { s_dal!.Order.DeleteAll(); }

    // delivery menu functions
    private static void addDelivery() 
    {
        Console.WriteLine(" ;-) אין מימוש לפונקציה");
    }
    private static void viewDelivery()
    {
        Console.WriteLine("Please enter delivery ID number you wish to display.");
        int? id = int.Parse(Console.ReadLine());
        if (id == null)
            throw new Exception(@"invalid delivery ID");
        Console.WriteLine(  s_dal.Delivery.Read((int)id));
    }
    private static void viewAllDeliveries() 
    {
        foreach (var item in s_dal.Delivery!.ReadAll())
        {
            Console.WriteLine(item.ToString());
        }
    }
    private static void updateDelivery() 
    {
        Console.WriteLine(" ;-) אין מימוש לפונקציה");

        Delivery delivery = new Delivery(
            // לקלוט מהמשתמש את כל שדות המשלוח לעדכון
            );
        s_dal.Delivery.Update(delivery); 
    }
    private static void deleteDelivery()
    {
        Console.WriteLine("Please enter delivery ID number you wish to delete.");
        int? id = int.Parse(Console.ReadLine());
        if (id == null)
            throw new Exception(@"invalid delivery ID");
        s_dal.Delivery.Delete((int)id);
    }
    private static void deleteAllDeliveries() { s_dal.Delivery.DeleteAll(); }
    //---------------------------------------------------------------------------------------------------------------------------------------
    // main mnue functions
    private static void courierMenu() 
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {

                Console.WriteLine(@$"Please enter a number to choose an action:
1-Exit
2-AddCourier
3-ViewCouriers
4-viewAllCourier
5-UpdateCourier
6-DeleteCourier
7-DeleteAllCouriers");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);

            switch ((CourierMenu)choice)
            {
                case CourierMenu.Exit:
                    continueLoop = false;
                    break;
                case CourierMenu.AddCourier:
                    addCourier();
                    break;
                case CourierMenu.ViewCourier:
                      viewCourier();
                        break;
                case CourierMenu.ViewAllCouriers:
                    viewAllCouriers();
                    break;
                case CourierMenu.UpdateCourier:
                    updateCourier();
                    break;
                case CourierMenu.DeleteCourier:
                    deleteCourier();
                    break;
                case CourierMenu.DeleteAllCouriers:
                    deleteAllCouriers();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
        } while (continueLoop);
        
       
    }
    private static void deliveryMenu() 
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
    private static void orderMenu() 
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {
                Console.WriteLine(@$"Please enter a number to choose an action:
                     1-Exit
                     2-AddOrder
                     3-ViewOrders
                     4-viewAllOrders
                     5-UpdateOrder
                     6-DeleteOrder
                     7-DeleteAllOrders");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((OrderMenu)choice)
            {
                case OrderMenu.Exit:
                    //endProgram
                    break;
                case OrderMenu.AddOrder:
                    addDelivery();
                    break;
                case OrderMenu.ViewOrders:
                    viewOrder();
                    break;
                case OrderMenu.ViewAllOrders:
                    viewAllOrders();
                    break;
                case OrderMenu.UpdateOrder:
                    updateOrder();
                    break;
                case OrderMenu.DeleteOrder:
                    deleteOrder();
                    break;
                case OrderMenu.DeleteAllOrders:
                    deleteAllOrders();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }
        }  while (continueLoop);
    }
    private static void configMenu() 
    {
        bool continueLoop = true;
        do
        {
            int? choice = null;
            do
            {
                Console.WriteLine(@$"Please enter a number to choose an action:
                     1-Exit
                     2-AddMiniuteToClock
                     3-AddHourToClock
                     4-AddDayToClock
                     5-AddMonthToClock
                     6-AddYearToClock
                     7-ViewClock
                     8-UpDateMaxRange
                     9-ViewMaxDeliveryDuration
                     7-ResetConfig");
                choice = int.Parse(Console.ReadLine()!);
            } while (choice is null);
            switch ((ConfigMenu)choice)
            {
                case ConfigMenu.Exit:
                    continueLoop = false;
                    break;
                //case ConfigMenu.AddMiniuteToClock:
                //    addMiniuteToClock();
                //    break;
                //case ConfigMenu.AddHourToClock:
                //    addHourToClock();
                //    break;
                //case ConfigMenu.AddDayToClock:
                //    addDayToClock();
                //    break;
                //case ConfigMenu.AddMonthToClock:
                //    addMonthToClock();
                //    break;
                //case ConfigMenu.AddYearToClock:
                //    addYearToClock();
                //    break;
                //case ConfigMenu.ViewClock:
                //    viewClock();
                //    break;
                //case ConfigMenu.UpDateMaxRange:
                //    upDateMaxRange();
                //    break;
                //case ConfigMenu.ViewMaxDeliveryDuration:
                //    viewMaxDeliveryDuration();
                //    break;
                //case ConfigMenu.ResetConfig:
                //    resetConfig();
                //    break;
                default:
                    Console.WriteLine();
                    break;
            }
        } while (continueLoop);
    }
    private static void initializationData() 
    {
        //Initialization.Do(s_dal); 
        Initialization.Do(); // stage 4 
        Console.WriteLine("Data initialization completed successfully.");
    }
    private static void viewData()
    {
        viewAllCouriers();
        viewAllOrders();
        viewAllDeliveries();  
    }
    private static void resetData()
    {
        s_dal.ResetDB();    
    }

    //---------------------------------------------------------------------------------------------------------------------------------------
    private static int mainMenu()
    {

        int? choice = null;
        do
        {
            Console.WriteLine(@"Please enter a number to choose an action:
0-Exit
1-CourierMenu
2-OrderMenu
3-DeliveryMenu
4-ConfigMenu
5-InitializationData
6-ViewData
7-ResetData");
            choice = int.Parse(Console.ReadLine()!);
        } while (choice is null);
        return (int)choice;
        
    }

    static void Main(string[] args)
    {
        try
        {

            //List<DO.Courier> courier=s_dalCourier!.ReadAll(); 
            //List<DO.Order> orders = s_dalOrder!.ReadAll();
            //List<DO.Delivery> deliveries = s_dalDelivery!.ReadAll();
            // gets user choice and call the suitable function
            
            int choice=mainMenu();
            while(choice!=0)
            {
                 switch ((MainMenu)choice)
                    {
                        case MainMenu.CourierMenu:
                            courierMenu();
                            break;
                        case MainMenu.OrderMenu:
                            orderMenu();
                            break;
                        case MainMenu.DeliveryMenu:
                            deliveryMenu();
                            break;
                        case MainMenu.ConfigMenu:
                            configMenu();
                            break;
                        case MainMenu.InitializationData:
                            initializationData();
                            break;
                        case MainMenu.ViewData:
                            viewData();
                            break;
                        case MainMenu.ResetData:
                            resetData();
                            break;
                        default:
                            Console.WriteLine();
                            break;
                    }
                choice = mainMenu();
            }
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during data initialization: {ex.Message}");
        }
        
    }


}
