using System;
using CafeteriaClient.Communication;
using CafeteriaClient.Models;

namespace CafeteriaClient.Operations
{
    public static class MenuOperations
    {
        public static void FetchMenuItems()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("MENU_FETCH");
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error fetching menu items", ex);
            }
        }

        public static void FetchMenuItemsWithRecommendation()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_WITH_RECOMMENDATION");
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error fetching menu items with recommendation", ex);
            }
        }
 public class MenuItem
    {
        public string MenuType { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public string FoodType { get; set; }  // New property for food type
    }
        public static void AddMenuItem()
{
    try
    {
        MenuItem newItem = ReadMenuItemDetails();
        string request = $"ADD {newItem.MenuType} {newItem.Name} {newItem.Price} {newItem.Availability} {newItem.FoodType}";
        string response = ServerCommunicator.SendCommandToServer(request);
        Console.WriteLine("Received from server: {0}", response);
    }
    catch (Exception ex)
    {
        HandleError("Error adding menu item", ex);
    }
}
public static void UpdateMenuItem()
{
    try
    {
        Console.Write("Enter the food item name to update: ");
        string itemName = Console.ReadLine().Trim();

        decimal price = ReadDecimalFromConsole("Enter the new price: ");
        
        int availability = ReadIntFromConsole("Enter new availability (1 for available, 0 for not available): ");
        while (availability != 0 && availability != 1)
        {
            availability = ReadIntFromConsole("Invalid availability. Enter 1 for available or 0 for not available: ");
        }

        // Wrap the itemName in quotes to handle multi-word names
        string request = $"UPDATE \"{itemName}\" {price} {availability}";
        string response = ServerCommunicator.SendCommandToServer(request);
        Console.WriteLine("Received from server: {0}", response);
    }
    catch (Exception ex)
    {
        HandleError("Error updating menu item", ex);
    }
}



        public static void DeleteMenuItem()
        {
            try
            {
                Console.Write("Enter the item ID to delete: ");
                int itemId = ReadIntFromConsole();

                string request = $"DELETE {itemId}";
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error deleting menu item", ex);
            }
        }

private static MenuItem ReadMenuItemDetails()
{
    Console.Write("Enter the menu type (Breakfast, Lunch, Dinner): ");
    string menuType = Console.ReadLine().Trim();

    Console.Write("Enter the food item name: ");
    string itemName = Console.ReadLine().Trim();

    decimal price = ReadDecimalFromConsole("Enter the price: ");

    int availability = ReadIntFromConsole("Enter availability (1 for available, 0 for not available): ");
    while (availability != 0 && availability != 1)
    {
        availability = ReadIntFromConsole("Invalid availability. Enter 1 for available or 0 for not available: ");
    }

    Console.Write("Enter the food type (Vegetarian, Non-Vegetarian, Eggetarian): ");
    string foodType = Console.ReadLine().Trim();

    return new MenuItem
    {
        MenuType = menuType,
        Name = itemName,
        Price = price,
        Availability = availability,
        FoodType = foodType
    };
}


        private static decimal ReadDecimalFromConsole(string prompt)
        {
            Console.Write(prompt);
            decimal value;
            while (!decimal.TryParse(Console.ReadLine().Trim(), out value))
            {
                Console.Write("Invalid input. Enter the value again: ");
            }
            return value;
        }

        private static int ReadIntFromConsole(string prompt = "Enter a number: ")
        {
            Console.Write(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine().Trim(), out value))
            {
                Console.Write("Invalid input. Enter a number: ");
            }
            return value;
        }

        private static void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
        }
    }

    public class MenuItem
    {
        public string MenuType { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
    }
}
