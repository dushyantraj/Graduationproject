using System;
using System.Collections.Generic;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class EmployeeMenu : IMenu
    {
        private readonly MenuOperations _menuOperations;
        private readonly Dictionary<string, Action> _menuActions;

        public EmployeeMenu()
        {
            _menuOperations = new MenuOperations();
            _menuActions = new Dictionary<string, Action>
            {
                { "1", () => _menuOperations.SelectFoodItemForNextDay() },
                { "2", FeedbackOperations.FillFeedbackForm },
                { "3", EmployeeOperations.FetchNotificationForEmployee },
                { "4", FeedbackOperations.DetailedFeedback },
                { "5", EmployeeOperations.FetchEmployeeNotifications },
                { "6", EmployeeOperations.UpdateProfile },
                { "7", Program.Logout }
            };
        }

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
            Console.WriteLine("Employee Menu:");
            Console.WriteLine("1. Select Food Item for Next Day");
            Console.WriteLine("2. Fill the Feedback Form");
            Console.WriteLine("3. View Notification");
            Console.WriteLine("4. Detailed Feedback");
            Console.WriteLine("5. Fetch Employee Notifications");
            Console.WriteLine("6. Update Profile");
            Console.WriteLine("7. Logout");
        }

        private string GetMenuChoice()
        {
            Console.Write("Enter your choice: ");
            return Console.ReadLine();
        }

        private bool HandleMenuChoice(string choice)
        {
            if (_menuActions.TryGetValue(choice, out var action))
            {
                action();
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
            }
            return choice != "7"; // Return false only if the choice is to logout
        }
    }
}
