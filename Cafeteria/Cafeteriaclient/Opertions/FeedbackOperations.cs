
using System;
using CafeteriaClient.Services;
using CafeteriaClient.Utilities;

namespace CafeteriaClient.Operations
{
    public static class FeedbackOperations
    {
        public static void ProvideFeedbackOnRollout()
        {
            try
            {
                Console.Write("Enter the Rollout ID to provide feedback: ");
                string rolloutId = Console.ReadLine().Trim();

                Console.Write("Enter feedback comments: ");
                string comments = Console.ReadLine().Trim();

                int rating = ReadRatingFromConsole();

                string request = $"{ServerCommands.SubmitFeedback} {rolloutId} {rating} \"{comments}\"";
                string response = ServerCommunicator.SendCommandToServer(request);
                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError($"Error providing feedback on rollout: {ex.Message}");
            }
        }

        public static void SendFeedbackForm()
        {
            try
            {
                EmployeeOperations.ViewEmployeeSelections();

                Console.Write("Enter food item for review: ");
                string foodItem = Console.ReadLine().Trim();

                string request = $"{ServerCommands.SendFeedbackForm} {foodItem}";
                string response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError($"Error sending feedback form: {ex.Message}");
            }
        }
        public static void FetchAvailableItemsForDetailedFeedback()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("CHECK_FEEDBACK_ROLLOUT");
                Console.WriteLine("Available items for detailed feedback:\n" + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching available items for feedback: {ex.Message}");
            }
        }

        public static void DetailedFeedback()
        {
            try
            {
                FetchAvailableItemsForDetailedFeedback();
                string foodItem = PromptUserForFoodItem();

                string feedbackQuestions = FetchFeedbackQuestions(foodItem);
                Console.WriteLine(feedbackQuestions);

                string[] responses = PromptUserForFeedbackResponses();

                string request = $"{ServerCommands.SubmitDetailedFeedback} \"{foodItem}\" \"{responses[0]}\" \"{responses[1]}\" \"{responses[2]}\"";
                string response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError($"Error submitting detailed feedback: {ex.Message}");
            }
        }

        public static void FillFeedbackForm()
        {
            try
            {
                string foodItem = PromptUserForFoodItem();

                int rating = ReadRatingFromConsole();

                Console.Write("Enter your comments: ");
                string comments = Console.ReadLine().Trim();

                string request = $"{ServerCommands.SubmitFeedback} \"{foodItem}\" {rating} \"{comments}\"";
                string response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                HandleError($"Error filling feedback form: {ex.Message}");
            }
        }

        private static int ReadRatingFromConsole()
        {
            Console.Write("Enter your rating (1 to 5): ");
            int rating;
            while (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
            {
                Console.Write("Invalid rating. Enter a rating between 1 and 5: ");
            }
            return rating;
        }

        private static string PromptUserForFoodItem()
        {
            Console.Write("Enter the Food Item: ");
            return Console.ReadLine().Trim();
        }

        private static bool CheckRolloutStatus(string foodItem)
        {
            string checkResponse = ServerCommunicator.SendCommandToServer($"{ServerCommands.CheckFeedbackRollout} \"{foodItem}\"");
            if (checkResponse.Contains("not currently rolled out"))
            {
                Console.WriteLine(checkResponse);
                return false;
            }
            return true;
        }

        private static string FetchFeedbackQuestions(string foodItem)
        {
            return ServerCommunicator.SendCommandToServer($"{ServerCommands.FetchDetailedFeedbackQuestions} \"{foodItem}\"");
        }

        private static string[] PromptUserForFeedbackResponses()
        {
            string[] responses = new string[3];

            Console.Write("Enter your response for Question 1: ");
            responses[0] = Console.ReadLine().Trim();

            Console.Write("Enter your response for Question 2: ");
            responses[1] = Console.ReadLine().Trim();

            Console.Write("Enter your response for Question 3: ");
            responses[2] = Console.ReadLine().Trim();

            return responses;
        }

        private static void HandleError(string errorMessage)
        {
            Console.WriteLine($"Error: {errorMessage}");
        }
    }
}
