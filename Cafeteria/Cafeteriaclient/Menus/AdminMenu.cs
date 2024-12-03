using System;
using System.Collections.Generic;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class AdminMenu : IMenu
    {
        private readonly MenuOperations _menuOperations;
        private readonly Dictionary<string, Action> _menuActions;

        public AdminMenu()
        {
            _menuOperations = new MenuOperations();
            _menuActions = new Dictionary<string, Action>
            {
                { "1", () => _menuOperations.FetchMenuItems() },
                { "2", () => _menuOperations.AddMenuItem() },
                { "3", () => _menuOperations.UpdateMenuItem() },
                { "4", () => _menuOperations.DeleteMenuItem() },
                { "5", () => _menuOperations.DiscardFoodItem() },
                { "6", Program.Logout }
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
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1. Fetch Menu Items");
            Console.WriteLine("2. Add Menu Item");
            Console.WriteLine("3. Update Menu Item");
            Console.WriteLine("4. Delete Menu Item");
            Console.WriteLine("5. Discard Menu Items");
            Console.WriteLine("6. Logout");
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
            return choice != "6"; // Return false only if the choice is to logout
        }
    }
}
