using System;
using CafeteriaClient.Operations;
using CafeteriaClient.Utilities;
namespace CafeteriaClient.Menus
{
    class EmployeeMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.WriteLine("Employee Menu:");
                Console.WriteLine("1. Select Food Item for Next Day");
                Console.WriteLine("2. Fill the Feedback Form");
                Console.WriteLine("3. View Notification");
                Console.WriteLine("4. Logout");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EmployeeOperations.SelectFoodItemForNextDay();
                        break;
                    case "2":
                        EmployeeOperations.FillFeedbackForm();
                        break;
                    case "3":
                        EmployeeOperations.FetchNotificaionForEmployee();
                        break;
                    case "4":
                        Utils.Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
