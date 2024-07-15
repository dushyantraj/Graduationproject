using System;
using CafeteriaClient.Utilities;
using CafeteriaClient.Services;
using System.Text;

namespace CafeteriaClient.Operations
{
    public class ChefOperations
    {
        private ServerCommunicator serverCommunicator;

        public ChefOperations()
        {
            serverCommunicator = new ServerCommunicator();
        }

        public void FetchNotificationForChef()
        {
            try
            {
                string response = serverCommunicator.SendCommandToServer(ServerCommands.FetchNotificationForChef);
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications for chef: {ex.Message}");
            }
        }

        public void RolloutFoodItemForNextDay()
        {
            try
            {
                var menuOperations = new MenuOperations();
                var rolloutManager = new RolloutManager();


                menuOperations.FetchMenuItemsWithRecommendation();

                string[] selectedItems = rolloutManager.GetItemSelectionsFromUser();

                rolloutManager.SendRolloutRequest(selectedItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling out food item for next day: {ex.Message}");
            }
        }
    }

    // Class to handle the rollout process
    public class RolloutManager
    {
        private ServerCommunicator serverCommunicator;

        public RolloutManager()
        {
            serverCommunicator = new ServerCommunicator();
        }

        public string[] GetItemSelectionsFromUser()
        {
            Console.WriteLine("Enter the Item IDs (separated by spaces) to roll out for the next day:");
            string itemIdsInput = Console.ReadLine();
            return itemIdsInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        public void SendRolloutRequest(string[] itemIds)
        {
            string request = BuildRolloutRequest(itemIds);
            string response = serverCommunicator.SendCommandToServer(request);
            Console.WriteLine("Received from server: {0}", response);
        }

        private string BuildRolloutRequest(string[] itemIds)
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
