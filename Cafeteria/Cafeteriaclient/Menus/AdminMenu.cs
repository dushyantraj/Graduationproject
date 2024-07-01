using System;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    public class AdminMenu : IMenu
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
            switch (choice)
            {
                case "1":
                    MenuOperations.FetchMenuItems();
                    break;
                case "2":
                    MenuOperations.AddMenuItem();
                    break;
                case "3":
                    MenuOperations.UpdateMenuItem();
                    break;
                case "4":
                    MenuOperations.DeleteMenuItem();
                    break;
                case "5":
                    ChefOperations.DiscardFoodItem();
                    break;
                case "6":
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
