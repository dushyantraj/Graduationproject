
using System;
using CafeteriaClient.Utilities;
using CafeteriaClient.Services;
using System.Text;
namespace CafeteriaClient.Operations
{
    public static class ChefOperations
    {
        public static void FetchNotificationForChef()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer(ServerCommands.FetchNotificationForChef);
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications for chef: {ex.Message}");
            }
        }

        public static void RolloutFoodItemForNextDay()
        {
            try
            {
                MenuOperations.FetchMenuItemsWithRecommendation();

                Console.WriteLine("Enter the Item IDs (separated by spaces) to roll out for the next day:");
                string itemIdsInput = Console.ReadLine();
                string[] itemIds = itemIdsInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                string request = BuildRolloutRequest(itemIds);
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling out food item for next day: {ex.Message}");
            }
        }

        private static string BuildRolloutRequest(string[] itemIds)
        {
            StringBuilder sb = new StringBuilder(ServerCommands.Rollout);

            foreach (var itemId in itemIds)
            {
                if (int.TryParse(itemId, out _))
                {
                    sb.Append(" ").Append(itemId);
                }
                else
                {
                    Console.WriteLine($"Invalid item ID format: {itemId}. Skipping.");
                }
            }

            return sb.ToString();
        }
    }
}
