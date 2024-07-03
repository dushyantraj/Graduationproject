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

            return new MenuItem
            {
                MenuType = menuType,
                Name = itemName,
                Price = price,
                Availability = availability,
                FoodType = foodType
            };
        }

        public static (string ItemName, decimal Price, int Availability) ReadUpdateDetails()
        {
            Console.Write("Enter the food item name to update: ");
            string itemName = Console.ReadLine().Trim();

            decimal price = ConsoleHelper.ReadDecimalFromConsole("Enter the new price: ");

            int availability = ConsoleHelper.ReadAvailability();

            return (ItemName: itemName, Price: price, Availability: availability);
        }
    }
}
