using Dal;
using DalApi;
using DO;
using System;
using System.Diagnostics;
using System.Linq;
namespace DalTest
{
   
    internal class Program
    {
        
        private static ICourier? s_dalCourier = new CourierImplementation(); //stage 1
        private static IDelivery? s_dalDelivery = new DeliveryImplementation(); //stage 1
        private static IOrder? s_dalOrder = new OrederImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
        //---------------------------------------------------------------------------------------------------------------------------------------
        // courier mnue functions
        private static void addCourier() 
        {
            Console.WriteLine("Please enter the following details:");

            Console.Write("ID number: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Full name: ");
            string fullName = Console.ReadLine() ?? "";

            Console.Write("Phone number: ");
            string phone = Console.ReadLine() ?? "";

            Console.Write("Email address: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            Console.Write("Is the employee active? (true/false): ");
            bool active = bool.Parse(Console.ReadLine() ?? "false");

            Console.Write("Maximum delivery distance (optional, press Enter to skip): ");
            string? distanceInput = Console.ReadLine();
            double? maxDistance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);

            Console.Write("Delivery type (enter number: 0-Car, 1-Motorcycle, 2-Drone, 3-Walking): ");
            DeliveryType deliveryType = (DeliveryType)int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Employment start date (YYYY-MM-DD): ");
            DateTime employmentStartDate = DateTime.Parse(Console.ReadLine() ?? DateTime.Now.ToString("yyyy-MM-dd"));

            Courier courier = new Courier()
            {
                Id = id,
                FullName = fullName,
                Phone = phone,
                Email = email,
                Password = password,
                Active = active,
                MaxDistance = maxDistance,
                DeliveryType = deliveryType,
                EmploymentStartDate = employmentStartDate
            };
            s_dalCourier.Create(courier);
            Console.WriteLine("Courier created successfully!");

        }
        private static void viewCourier()
        {
            Console.WriteLine("Please enter  ID number of courier you wish to display.");
            int? id= int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid ID");
            Console.WriteLine(s_dalCourier.Read((int)id)); 

        }
        private static void viewAllCouriers()
        {
            var courier = s_dalCourier!.ReadAll().ToArray();
            for (int i = 0; i < s_dalCourier.ReadAll().Count; i++)
            {
                Console.WriteLine(courier[i].ToString());
            }
        }
        private static void updateCourier(Courier courier) { s_dalCourier.Update(courier); }
        private static void deleteCourier()
        {
            Console.WriteLine("Please enter delivery ID number of coureir you wish to delete.");
            int? id = int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid delivery ID");
            s_dalDelivery.Delete((int)id);
        }
        private static void deleteAllCouriers() { s_dalCourier.DeleteAll(); }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // order mnue functions
        private static void addOrder() { }
        private static void viewOrder() 
        {
            Console.WriteLine("Please enter order ID number you wish to display.");
            int? id = int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid order ID");
            Console.WriteLine(s_dalOrder.Read((int)id));
        }
        private static void viewAllOrders() 
        {
            var order = s_dalOrder!.ReadAll().ToArray();
            for (int i = 0; i < s_dalOrder.ReadAll().Count; i++)
            {
                Console.WriteLine(order[i].ToString());
            }
        }
        private static void updateOrder(Order order) { s_dalOrder.Update(order); }
        private static void deleteOrder()
        {
            Console.WriteLine("Please enter order ID number you wish to deiete.");
            int? id = int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid order ID");
           s_dalOrder.Delete((int)id);
        }
        private static void deleteAllOrders() { s_dalOrder.DeleteAll(); }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // delivery menu functions
        private static void addDelivery() { }
        private static void viewDelivery()
        {
            Console.WriteLine("Please enter delivery ID number you wish to display.");
            int? id = int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid delivery ID");
            Console.WriteLine(  s_dalDelivery.Read((int)id));
        }
        private static void viewAllDeliveries() 
        {
            var delivery = s_dalDelivery!.ReadAll().ToArray();
            for (int i = 0; i < s_dalDelivery.ReadAll().Count; i++)
            {
                Console.WriteLine(delivery[i].ToString());
            }
        }
        private static void updateDelivery(Delivery delivery) { s_dalDelivery.Update(delivery); }
        private static void deleteDelivery()
        {
            Console.WriteLine("Please enter delivery ID number you wish to delete.");
            int? id = int.Parse(Console.ReadLine());
            if (id == null)
                throw new Exception(@"invalid delivery ID");
            s_dalDelivery.Delete((int)id);
        }
        private static void deleteAllDeliveries() { s_dalDelivery.DeleteAll(); }
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
                        {
                          viewCourier();
                            break;
                        }
                        
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
        { Initialization.Do(s_dalConfig, s_dalCourier, s_dalDelivery, s_dalOrder);
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
            s_dalConfig!.Reset();
            s_dalCourier.DeleteAll();
            s_dalDelivery!.DeleteAll();
            s_dalOrder!.DeleteAll();
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
                List<DO.Courier> courier=s_dalCourier!.ReadAll(); 
                List<DO.Order> orders = s_dalOrder!.ReadAll();
                List<DO.Delivery> deliveries = s_dalDelivery!.ReadAll();
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

}
