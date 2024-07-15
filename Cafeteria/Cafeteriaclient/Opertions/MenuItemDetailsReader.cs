using System;
using CafeteriaClient.Models;

namespace CafeteriaClient.Utilities
{
    public static class MenuItemDetailsReader
    {
        public static MenuItem ReadMenuItemDetails()
{
    Console.Write("Enter the menu type (Breakfast, Lunch, Dinner): ");
    string menuType = Console.ReadLine().Trim();

    Console.Write("Enter the food item name: ");
    string itemName = Console.ReadLine().Trim();

    decimal price = ConsoleHelper.ReadDecimalFromConsole("Enter the price: ");

    int availability = ConsoleHelper.ReadAvailability();

    Console.Write("Enter the food type (Vegetarian, Non-Vegetarian, Eggetarian): ");
    string foodType = Console.ReadLine().Trim();

    Console.Write("Enter the cuisine preference (North Indian, South Indian, Other): ");
    string cuisinePreference = Console.ReadLine().Trim();

    Console.Write("Enter the spice level (High, Medium, Low): ");
    string spiceLevel = Console.ReadLine().Trim();

    return new MenuItem
    {
        MenuType = menuType,
        Name = itemName,
        Price = price,
        Availability = availability,
        FoodType = foodType,
        CuisinePreference = cuisinePreference,
        SpiceLevel = spiceLevel
    };
}


        public static (string ItemName, decimal Price, int Availability, string SpiceLevel) ReadUpdateDetails()
        {
            Console.Write("Enter the food item name to update: ");
            string itemName = Console.ReadLine().Trim();

            decimal price = ConsoleHelper.ReadDecimalFromConsole("Enter the new price: ");
            int availability = ConsoleHelper.ReadAvailability();

            Console.Write("Enter the spice level (High, Medium, Low): ");
            string spiceLevel;
            while (true)
            {
                spiceLevel = Console.ReadLine().Trim();
                if (spiceLevel.Equals("High", StringComparison.OrdinalIgnoreCase) ||
                    spiceLevel.Equals("Medium", StringComparison.OrdinalIgnoreCase) ||
                    spiceLevel.Equals("Low", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                Console.Write("Invalid input. Please enter spice level as High, Medium, or Low: ");
            }

            return (ItemName: itemName, Price: price, Availability: availability, SpiceLevel: spiceLevel);
        }

    }
}
