using System;
using CafeteriaClient.Operations;

namespace CafeteriaClient.Menus
{
    class AdminMenu
    {
        public static void Show()
        {
            while (true)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Fetch Menu Items");
                Console.WriteLine("2. Add Menu Item");
                Console.WriteLine("3. Update Menu Item");
                Console.WriteLine("4. Delete Menu Item");
                Console.WriteLine("5. Logout");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

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
