using System;
using System.Text;
using CafeteriaClient.Communication;

namespace CafeteriaClient.Operations
{
    public static class ChefOperations
    {
        public static void FetchNotificationForChef()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_NOTIFICATION_FOR_CHEF");
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

                StringBuilder sb = new StringBuilder();
                sb.Append("ROLLOUT");

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

                string request = sb.ToString();
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling out food item for next day: {ex.Message}");
            }
        }

        public static void ViewEmployeeSelections()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_EMPLOYEE_SELECTIONS");
                Console.WriteLine("Employee Selections:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching employee selections: {ex.Message}");
            }
        }

        public static void ProvideFeedbackOnRollout()
        {
            try
            {
                Console.Write("Enter the Rollout ID to provide feedback: ");
                string rolloutId = Console.ReadLine();

                Console.Write("Enter feedback comments: ");
                string comments = Console.ReadLine();

                Console.Write("Enter rating (1 to 5): ");
                int rating;
                while (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
                {
                    Console.Write("Invalid rating. Enter a rating between 1 and 5: ");
                }

                string request = $"FEEDBACK {rolloutId} {rating} \"{comments}\"";
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error providing feedback on rollout: {ex.Message}");
            }
        }

        public static void SendFeedbackForm()
        {
            try
            {
                ViewEmployeeSelections();

                Console.Write("Enter food item for review: ");
                string foodItem = Console.ReadLine();

                string request = $"SEND_FEEDBACK_FORM {foodItem}";
                string response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending feedback form: {ex.Message}");
            }
        }
        public static void DiscardFoodItem()
        {
            try
            {
                Console.WriteLine("Fetching Discard Menu Item List...");
                string discardList = ServerCommunicator.SendCommandToServer("FETCH_DISCARD_MENU_ITEMS");
                Console.WriteLine(discardList);

                Console.WriteLine("Options:");
                Console.WriteLine("1) Remove a Food Item from Menu");
                Console.WriteLine("2) Get Detailed Feedback on a Food Item");
                Console.WriteLine("3) Exit");

                string subChoice = Console.ReadLine();

                switch (subChoice)
                {
                    case "1":
                        Console.WriteLine("Enter the name of the food item to remove:");
                        string removeItemName = Console.ReadLine();
                        string removeRequest = $"REMOVE_DISCARD_MENU_ITEM {removeItemName}";
                        string removeResponse = ServerCommunicator.SendCommandToServer(removeRequest);
                        Console.WriteLine(removeResponse);
                        break;

                    case "2":
                        Console.WriteLine("Enter the name of the food item for feedback:");
                        string feedbackItemName = Console.ReadLine();
                        string feedbackRequest = $"ROLL_OUT_FEEDBACK {feedbackItemName}";
                        string feedbackResponse = ServerCommunicator.SendCommandToServer(feedbackRequest);
                        Console.WriteLine(feedbackResponse);
                        break;

                    case "3":
                        Console.WriteLine("Exiting Discard Food Item menu.");
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error managing discard food items: {ex.Message}");
            }
        }
    }
}
