using System;
using CafeteriaClient.Communication;

namespace CafeteriaClient.Operations
{
    class EmployeeOperations
    {
        public static void FetchNotificaionForEmployee()
        {
            string response = ServerCommunicator.SendCommandToServer("FETCH_NOTIFICATION_FOR_EMPLOYEE");
            Console.WriteLine("Received from server:\n{0}", response);
        }

        public static void SelectFoodItemForNextDay()
        {
            string response = ServerCommunicator.SendCommandToServer("FETCH_ROLLOUT");
            Console.WriteLine("Available items for the next day:\n" + response);

            Console.Write("Enter the Rollout ID of the item you want to select: Breakfast, Lunch, Dinner:,");
            string rolloutId = Console.ReadLine();

            string request = $"SELECT_ITEM {rolloutId}";
            response = ServerCommunicator.SendCommandToServer(request);
            Console.WriteLine("Received from server: " + response);
        }

        public static void FillFeedbackForm()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_FEEDBACK");
                Console.WriteLine("Feedback Form:\n" + response);

                Console.Write("Enter the Food Item for feedback: ");
                string foodItem = Console.ReadLine().Trim();

                Console.Write("Enter your rating (1 to 5): ");
                int rating;
                while (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
                {
                    Console.Write("Invalid rating. Please enter a number between 1 and 5: ");
                }

                Console.Write("Enter your comments: ");
                string comments = Console.ReadLine().Trim();

                string request = $"SUBMIT_FEEDBACK \"{foodItem}\" {rating} \"{comments}\"";
                response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error filling feedback form: " + ex.Message);
            }
        }
    }
}
