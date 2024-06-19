using System;
using System.Text;
using CafeteriaClient.Communication;

namespace CafeteriaClient.Operations
{
    class ChefOperations
    {
        public static void RolloutFoodItemForNextDay()
        {
            MenuOperations.FetchMenuItems(); // Display current menu items

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

        public static void ViewEmployeeSelections()
        {
            string response = ServerCommunicator.SendCommandToServer("FETCH_EMPLOYEE_SELECTIONS");
            Console.WriteLine("Employee Selections:\n{0}", response);
        }

        public static void ProvideFeedbackOnRollout()
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
                Console.WriteLine("Error sending feedback form: " + ex.Message);
            }
        }
    }
}
