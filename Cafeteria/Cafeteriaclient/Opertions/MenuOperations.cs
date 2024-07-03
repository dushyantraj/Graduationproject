using System;
using CafeteriaClient.Models;
using CafeteriaClient.Utilities;
using CafeteriaClient.Services;

namespace CafeteriaClient.Operations
{
    public class MenuOperations
    {
        private readonly ServerCommunicator serverCommunicator;

        public MenuOperations()
        {
            serverCommunicator = new ServerCommunicator();
        }

        public void FetchMenuItems()
        {
            try
            {
                string response = serverCommunicator.SendCommandToServer(ServerCommands.FetchMenuItems);
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error fetching menu items", ex);
            }
        }

        public void SelectFoodItemForNextDay()
        {
            try
            {
                if (string.IsNullOrEmpty(Program.CurrentUsername))
                {
                    Console.WriteLine("Error: No user is currently logged in.");
                    return;
                }

                // Fetch available items for the next day
                FetchAvailableItemsForNextDay();

                // Get user's selection
                string rolloutId = GetUserRolloutSelection();

                // Process the user's selection
                ProcessRolloutSelection(rolloutId);
            }
            catch (Exception ex)
            {
                HandleError("Error selecting food item for next day", ex);
            }
        }

        public void FetchMenuItemsWithRecommendation()
        {
            try
            {
                string response = serverCommunicator.SendCommandToServer(ServerCommands.FetchMenuWithRecommendation);
                Console.WriteLine("Received from server with recommendations:\n{0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error fetching menu items with recommendation", ex);
            }
        }

        public void AddMenuItem()
        {
            try
            {
                var newItem = MenuItemDetailsReader.ReadMenuItemDetails();
                string request = $"{ServerCommands.AddMenuItem} {newItem.MenuType} {newItem.Name} {newItem.Price} {newItem.Availability} {newItem.FoodType}";
                string response = serverCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error adding menu item", ex);
            }
        }

        public void UpdateMenuItem()
        {
            try
            {
                var updateDetails = MenuItemDetailsReader.ReadUpdateDetails();
                string request = $"{ServerCommands.UpdateMenuItem} {updateDetails.ItemName} {updateDetails.Price} {updateDetails.Availability} {updateDetails.SpiceLevel}";
                string response = serverCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error updating menu item", ex);
            }
        }

        public void DeleteMenuItem()
        {
            try
            {
                int itemId = ConsoleHelper.ReadIntFromConsole("Enter the item ID to delete: ");
                string request = $"{ServerCommands.DeleteMenuItem} {itemId}";
                string response = serverCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError("Error deleting menu item", ex);
            }
        }

        public void DiscardFoodItem()
        {
            try
            {
                string discardList = serverCommunicator.SendCommandToServer(ServerCommands.FetchDiscardMenuItems);
                Console.WriteLine("Fetching Discard Menu Item List...\n{0}", discardList);

                DiscardMenuHandler.HandleDiscardMenu();
            }
            catch (Exception ex)
            {
                HandleError("Error managing discard food items", ex);
            }
        }

        public void FetchAvailableItemsForNextDay()
        {
            try
            {
                string request = $"{ServerCommands.FetchRollout} {Program.CurrentUsername}";
                string response = serverCommunicator.SendCommandToServer(request);
                Console.WriteLine("Available items for the next day:\n" + response);
            }
            catch (Exception ex)
            {
                HandleError($"Error fetching available items for the next day: {ex.Message}", ex);
            }
        }

        public void ProcessRolloutSelection(string userSelection)
        {
            try
            {
                string request = $"{ServerCommands.SelectItem} {Program.CurrentUsername} {userSelection}";
                string response = serverCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: " + response);
            }
            catch (Exception ex)
            {
                HandleError($"Error processing rollout selection: {ex.Message}", ex);
            }
        }

        public string GetUserRolloutSelection()
        {
            Console.Write("Enter the Rollout ID of the item you want to select: ");
            return Console.ReadLine();
        }

        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"{message}: {ex.Message}");
        }
    }
}
