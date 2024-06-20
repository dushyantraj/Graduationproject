using System;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    class ChefMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.WriteLine("Chef Menu:");
                Console.WriteLine("1. Fetch Menu Items");
                Console.WriteLine("2. Rollout Food Item for Next Day");
                Console.WriteLine("3. View Employee Selections for Next Day");
                Console.WriteLine("4. Monthly Report");
                Console.WriteLine("5. Send Feedback Form to Employees");
                Console.WriteLine("6. Logout");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        MenuOperations.FetchMenuItems();
                        break;
                    case "2":
                        ChefOperations.RolloutFoodItemForNextDay();
                        break;
                    case "3":
                        ChefOperations.ViewEmployeeSelections();
                        break;
                    case "4":
                        ChefOperations.ProvideFeedbackOnRollout();
                        break;
                    case "5":
                        ChefOperations.SendFeedbackForm();
                        break;
                    case "6":
                        Program.Logout();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}
