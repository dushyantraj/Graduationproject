using System;
using System.Collections.Generic;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class ChefMenu : IMenu
    {
        private readonly MenuOperations _menuOperations;
        private readonly ChefOperations _chefOperations;
        private readonly Dictionary<string, Action> _menuActions;

        public ChefMenu()
        {
            _menuOperations = new MenuOperations();
            _chefOperations = new ChefOperations();
            _menuActions = new Dictionary<string, Action>
            {
                { "1", () => _menuOperations.FetchMenuItems() },
                { "2", () => _chefOperations.RolloutFoodItemForNextDay() },
                { "3", EmployeeOperations.ViewEmployeeSelections },
                { "4", () => _chefOperations.FetchNotificationForChef() },
                { "5", FeedbackOperations.SendFeedbackForm },
                { "6", _menuOperations.DiscardFoodItem },
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
            if (_menuActions.TryGetValue(choice, out var action))
            {
                action();
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
            }
            return choice != "7";
        }
    }
}
