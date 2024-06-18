using System;
using MenuClient.Communication;

namespace MenuClient.Operations
{
    class MenuOperations
    {
        public static void FetchMenuItems()
        {
            string response = ServerCommunicator.SendCommandToServer("FETCH");
            Console.WriteLine("Received from server:\n{0}", response);
        }

        public static void AddMenuItem()
        {
            Console.Write("Enter the menu type (Breakfast, Lunch, Dinner): ");
            string menuType = Console.ReadLine().Trim();

            Console.Write("Enter the food item name: ");
            string itemName = Console.ReadLine().Trim();

            Console.Write("Enter the price: ");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine().Trim(), out price))
            {
                Console.Write("Invalid price format. Enter the price again: ");
            }

            Console.Write("Enter availability (1 for available, 0 for not available): ");
            int available;
            while (!int.TryParse(Console.ReadLine().Trim(), out available) || (available != 0 && available != 1))
            {
                Console.Write("Invalid availability. Enter 1 for available or 0 for not available: ");
            }

            string request = $"ADD {menuType} {itemName} {price} {available}";
            string response = ServerCommunicator.SendCommandToServer(request);
            Console.WriteLine("Received from server: {0}", response);
        }

        public static void UpdateMenuItem()
        {
            Console.Write("Enter the item ID to update: ");
            int itemId;
            while (!int.TryParse(Console.ReadLine().Trim(), out itemId))
            {
                Console.Write("Invalid item ID. Enter the item ID again: ");
            }

            Console.Write("Enter the new menu type (Breakfast, Lunch, Dinner): ");
            string menuType = Console.ReadLine().Trim();

            Console.Write("Enter the new food item name: ");
            string itemName = Console.ReadLine().Trim();

            Console.Write("Enter the new price: ");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine().Trim(), out price))
            {
                Console.Write("Invalid price format. Enter the price again: ");
            }

            Console.Write("Enter the new availability (1 for available, 0 for not available): ");
            int available;
            while (!int.TryParse(Console.ReadLine().Trim(), out available) || (available != 0 && available != 1))
            {
                Console.Write("Invalid availability. Enter 1 for available or 0 for not available: ");
            }

            string request = $"UPDATE {itemId} {menuType} {itemName} {price} {available}";
            string response = ServerCommunicator.SendCommandToServer(request);
            Console.WriteLine("Received from server: {0}", response);
        }

        public static void DeleteMenuItem()
        {
            Console.Write("Enter the item ID to delete: ");
            int itemId;
            while (!int.TryParse(Console.ReadLine().Trim(), out itemId))
            {
                Console.Write("Invalid item ID. Enter the item ID again: ");
            }

            string request = $"DELETE {itemId}";
            string response = ServerCommunicator.SendCommandToServer(request);
            Console.WriteLine("Received from server: {0}", response);
        }
    }
}
