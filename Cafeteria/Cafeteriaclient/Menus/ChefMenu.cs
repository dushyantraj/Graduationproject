using System;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class ChefMenu : IMenu
    {
        public void Show()
        {
            while (true)
            {
                DisplayMenu();
                string choice = GetMenuChoice();

                if (!HandleMenuChoice(choice))
                {
                    break;
                }
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("Chef Menu:");
            Console.WriteLine("1. Fetch Menu Items");
            Console.WriteLine("2. Rollout Food Item for Next Day");
            Console.WriteLine("3. View Employee Selections for Next Day");
            Console.WriteLine("4. View Notifications");
            Console.WriteLine("5. Send Feedback Form to Employees");
            Console.WriteLine("6. Discard Items");
            Console.WriteLine("7. Logout");
        }

        private string GetMenuChoice()
        {
            Console.Write("Enter your choice: ");
            return Console.ReadLine();
        }

        private bool HandleMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    MenuOperations.FetchMenuItems();
                    break;
                case "2":
                    ChefOperations.RolloutFoodItemForNextDay();
                    break;
                case "3":
                    EmployeeOperations.ViewEmployeeSelections();
                    break;
                case "4":
                    ChefOperations.FetchNotificationForChef();
                    break;
                case "5":
                    FeedbackOperations.SendFeedbackForm();
                    break;
                case "6":
                    MenuOperations.DiscardFoodItem();
                    break;
                case "7":
                    Program.Logout();
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            return true;
        }
    }
}
