
using System;
using CafeteriaClient.Services;
using CafeteriaClient.Models;
using CafeteriaClient.Utilities;

namespace CafeteriaClient.Operations
{
    public static class MenuOperations
    {
        public static void SelectFoodItemForNextDay()
        {
            try
            {
                if (string.IsNullOrEmpty(Program.CurrentUsername))
                {
                    Console.WriteLine("Error: No user is currently logged in.");
                    return;
                }

                string request = $"{ServerCommands.FetchMenuWithRecommendation} {Program.CurrentUsername}";
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Available items for the next day:\n" + response);

                Console.Write("Enter the Rollout ID of the item you want to select: ");
                string rolloutId = Console.ReadLine();

                request = $"{ServerCommands.SelectItem} {Program.CurrentUsername} {rolloutId}";
                response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: " + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting food item for next day: {ex.Message}");
            }
        }

        public static void FetchMenuItems()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer(ServerCommands.FetchMenuItems);
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
                string response = ServerCommunicator.SendCommandToServer(ServerCommands.FetchMenuWithRecommendation);
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error fetching menu items with recommendation", ex);
            }
        }

        public static void AddMenuItem()
        {
            try
            {
                MenuItem newItem = ReadMenuItemDetails();
                string request = $"{ServerCommands.AddMenuItem} {newItem.MenuType} {newItem.Name} {newItem.Price} {newItem.Availability} {newItem.FoodType}";
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
                string request = $"{ServerCommands.UpdateMenuItem} \"{itemName}\" {price} {availability}";
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

                string request = $"{ServerCommands.DeleteMenuItem} {itemId}";
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

        public static void DiscardFoodItem()
        {
            try
            {
                Console.WriteLine("Fetching Discard Menu Item List...");
                string discardList = ServerCommunicator.SendCommandToServer(ServerCommands.FetchDiscardMenuItems);
                Console.WriteLine(discardList);

                DisplayDiscardMenuOptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error managing discard food items: {ex.Message}");
            }
        }

        private static void DisplayDiscardMenuOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1) Remove a Food Item from Menu");
            Console.WriteLine("2) Get Detailed Feedback on a Food Item");
            Console.WriteLine("3) Exit");

            string subChoice = Console.ReadLine();

            switch (subChoice)
            {
                case "1":
                    RemoveFoodItemFromMenu();
                    break;

                case "2":
                    GetDetailedFeedbackOnFoodItem();
                    break;

                case "3":
                    Console.WriteLine("Exiting Discard Food Item menu.");
                    break;

                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }

        private static void RemoveFoodItemFromMenu()
        {
            Console.WriteLine("Enter the name of the food item to remove:");
            string removeItemName = Console.ReadLine();
            string removeRequest = $"{ServerCommands.RemoveDiscardMenuItem} {removeItemName}";
            string removeResponse = ServerCommunicator.SendCommandToServer(removeRequest);
            Console.WriteLine(removeResponse);
        }

        private static void GetDetailedFeedbackOnFoodItem()
        {
            Console.WriteLine("Enter the name of the food item for feedback:");
            string feedbackItemName = Console.ReadLine();
            string feedbackRequest = $"{ServerCommands.RollOutFeedback} {feedbackItemName}";
            string feedbackResponse = ServerCommunicator.SendCommandToServer(feedbackRequest);
            Console.WriteLine(feedbackResponse);
        }
    }
}
