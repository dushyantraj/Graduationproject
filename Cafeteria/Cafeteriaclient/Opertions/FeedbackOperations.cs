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
                string rolloutId = PromptUserForInput("Enter the Rollout ID to provide feedback: ");
                string comments = PromptUserForInput("Enter feedback comments: ");
                int rating = ReadRatingFromConsole();

                // Create an instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string request = BuildFeedbackRequest(ServerCommands.SubmitFeedback, rolloutId, rating, comments);
                string response = serverCommunicator.SendCommandToServer(request);

                DisplayResponse("feedback on rollout", response);
            }
            catch (Exception ex)
            {
                HandleError("providing feedback on rollout", ex);
            }
        }

        public static void SendFeedbackForm()
        {
            try
            {
                EmployeeOperations.ViewEmployeeSelections();

                string foodItem = PromptUserForInput("Enter food item for review: ");

                // Create an instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string request = BuildSimpleRequest(ServerCommands.SendFeedbackForm, foodItem);
                string response = serverCommunicator.SendCommandToServer(request);

                DisplayResponse("sending feedback form", response);
            }
            catch (Exception ex)
            {
                HandleError("sending feedback form", ex);
            }
        }

        public static void FetchAvailableItemsForDetailedFeedback()
        {
            try
            {
                // Create an instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string response = serverCommunicator.SendCommandToServer(ServerCommands.CheckFeedbackRollout);
                DisplayResponse("available items for detailed feedback", response);
            }
            catch (Exception ex)
            {
                HandleError("fetching available items for detailed feedback", ex);
            }
        }

        public static void DetailedFeedback()
        {
            try
            {
                FetchAvailableItemsForDetailedFeedback();
                string foodItem = PromptUserForFoodItem();

                if (!CheckRolloutStatus(foodItem)) return;

                string feedbackQuestions = FetchFeedbackQuestions(foodItem);
                Console.WriteLine(feedbackQuestions);

                string[] responses = PromptUserForFeedbackResponses();

                // Create an instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string request = BuildDetailedFeedbackRequest(foodItem, responses);
                string response = serverCommunicator.SendCommandToServer(request);

                DisplayResponse("detailed feedback", response);
            }
            catch (Exception ex)
            {
                HandleError("submitting detailed feedback", ex);
            }
        }

        public static void FillFeedbackForm()
        {
            try
            {
                var serverCommunicator = new ServerCommunicator();
                string response = serverCommunicator.SendCommandToServer("FETCH_FEEDBACK");
                Console.WriteLine("Feedback Form:\n" + response);
                string foodItem = PromptUserForFoodItem();
                int rating = ReadRatingFromConsole();
                string comments = PromptUserForInput("Enter your comments: ");



                string request = BuildFeedbackRequest(ServerCommands.SubmitFeedback, foodItem, rating, comments);
                response = serverCommunicator.SendCommandToServer(request);

                DisplayResponse("filling feedback form", response);
            }
            catch (Exception ex)
            {
                HandleError("filling feedback form", ex);
            }
        }

        // Helper Methods
        private static string PromptUserForInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
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
            return PromptUserForInput("Enter the Food Item: ");
        }

        private static string[] PromptUserForFeedbackResponses()
        {
            string[] responses = new string[3];
            for (int i = 0; i < 3; i++)
            {
                responses[i] = PromptUserForInput($"Enter your response for Question {i + 1}: ");
            }
            return responses;
        }

        private static bool CheckRolloutStatus(string foodItem)
        {
            // Create an instance of ServerCommunicator
            var serverCommunicator = new ServerCommunicator();
            string checkResponse = serverCommunicator.SendCommandToServer($"{ServerCommands.CheckFeedbackRollout} \"{foodItem}\"");
            if (checkResponse.Contains("not currently rolled out"))
            {
                Console.WriteLine(checkResponse);
                return false;
            }
            return true;
        }

        private static string FetchFeedbackQuestions(string foodItem)
        {
            // Create an instance of ServerCommunicator
            var serverCommunicator = new ServerCommunicator();
            return serverCommunicator.SendCommandToServer($"{ServerCommands.FetchDetailedFeedbackQuestions} \"{foodItem}\"");
        }

        private static string BuildFeedbackRequest(string command, string item, int rating, string comments)
        {
            return $"{command} {item} {rating} \"{comments}\"";
        }

        private static string BuildSimpleRequest(string command, string parameter)
        {
            return $"{command} {parameter}";
        }

        private static string BuildDetailedFeedbackRequest(string foodItem, string[] responses)
        {
            return $"{ServerCommands.SubmitDetailedFeedback} \"{foodItem}\" \"{responses[0]}\" \"{responses[1]}\" \"{responses[2]}\"";
        }

        private static void DisplayResponse(string actionDescription, string response)
        {
            Console.WriteLine($"Received from server ({actionDescription}): {response}");
        }

        private static void HandleError(string action, Exception ex)
        {
            Console.WriteLine($"Error {action}: {ex.Message}");
        }
    }
}
