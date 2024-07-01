using System;
using CafeteriaClient.Communication;


namespace CafeteriaClient.Operations
{
    public static class EmployeeOperations
    {
        public static void FetchNotificationForEmployee()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_NOTIFICATION_FOR_EMPLOYEE");
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications for employee: {ex.Message}");
            }
        }
        public static void FetchAvailableItemsForFeedback()
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
                FetchAvailableItemsForFeedback();
                Console.Write("Enter the Food Item for detailed feedback: ");
                string foodItem = Console.ReadLine().Trim();
                string checkResponse = ServerCommunicator.SendCommandToServer($"CHECK_FEEDBACK_ROLLOUT \"{foodItem}\"");
                if (checkResponse.Contains("not currently rolled out"))
                {
                    Console.WriteLine(checkResponse);
                    return;
                }
                string feedbackQuestions = ServerCommunicator.SendCommandToServer($"FETCH_DETAILED_FEEDBACK_QUESTIONS \"{foodItem}\"");
                Console.WriteLine(feedbackQuestions);
                Console.Write("Enter your response for Question 1: ");
                string question1Response = Console.ReadLine().Trim();

                Console.Write("Enter your response for Question 2: ");
                string question2Response = Console.ReadLine().Trim();

                Console.Write("Enter your response for Question 3: ");
                string question3Response = Console.ReadLine().Trim();
                string request = $"SUBMIT_DETAILED_FEEDBACK \"{foodItem}\" \"{question1Response}\" \"{question2Response}\" \"{question3Response}\"";
                string response = ServerCommunicator.SendCommandToServer(request);

                Console.WriteLine("Received from server: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting detailed feedback: {ex.Message}");
            }
        }
        private static string EscapeQuotes(string input)
        {
            return input.Replace("\"", "\\\"");
        }


        public static void SelectFoodItemForNextDay()
{
    try
    {
        if (string.IsNullOrEmpty(Program.CurrentUsername))
        {
            Console.WriteLine("Error: No user is currently logged in.");
            return;
        }

        string request = $"FETCH_ROLLOUT {Program.CurrentUsername}";
        string response = ServerCommunicator.SendCommandToServer(request);
        Console.WriteLine("Available items for the next day:\n" + response);

        Console.Write("Enter the Rollout ID of the item you want to select: ");
        string rolloutId = Console.ReadLine();

        request = $"SELECT_ITEM {Program.CurrentUsername} {rolloutId}";
        response = ServerCommunicator.SendCommandToServer(request);
        Console.WriteLine("Received from server: " + response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error selecting food item for next day: {ex.Message}");
    }
}


        public static void FillFeedbackForm()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("CHECK_FEEDBACK_ROLLOUT");
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
                Console.WriteLine($"Error filling feedback form: {ex.Message}");
            }
        }
        public static void FetchEmployeeNotifications()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer("FETCH_NOTIFICATIONS");
                Console.WriteLine("Your Notifications:\n" + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
            }
        }

        // public static void UpdateProfile()
        // {
        //     try
        //     {
        //         Console.WriteLine("Please answer these questions to know your preferences:");

        // Console.WriteLine("1) Please select one:\n- Vegetarian\n- Non Vegetarian\n- Eggetarian");
        //         string preference = Console.ReadLine().Trim();

        //         Console.WriteLine("2) Please select your spice level:\n- High\n- Medium\n- Low");
        //         string spiceLevel = Console.ReadLine().Trim();

        //         Console.WriteLine("3) What do you prefer most?\n- North Indian\n- South Indian\n- Other");
        //         string cuisinePreference = Console.ReadLine().Trim();

        //         Console.WriteLine("4) Do you have a sweet tooth?\n- Yes\n- No");
        //         string sweetToothResponse = Console.ReadLine().Trim();
        //         bool sweetTooth = sweetToothResponse.Equals("Yes", StringComparison.OrdinalIgnoreCase);

        //         if (string.IsNullOrEmpty(Program.CurrentUsername))
        //         {
        //             Console.WriteLine("Error: No user is currently logged in.");
        //             return;
        //         }

        //         string request = $"UPDATE_PROFILE {Program.CurrentUsername} {preference} {spiceLevel} {cuisinePreference} {sweetTooth}";
        //         string response = ServerCommunicator.SendCommandToServer(request);

        //         Console.WriteLine(response);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error updating profile: {ex.Message}");
        //     }
        // }
public static void UpdateProfile()
{
    try
    {
        Console.WriteLine("Please answer these questions to know your preferences:");

        Console.WriteLine("1) Please select one:\n- Vegetarian\n- Non Vegetarian\n- Eggetarian");
        string preference = Console.ReadLine().Trim();

        // Ensure exact match for ENUM values
        string[] validPreferences = { "Vegetarian", "Non Vegetarian", "Eggetarian" };
        if (!validPreferences.Contains(preference))
        {
            Console.WriteLine($"Error: Invalid value for Preference: '{preference}'.");
            return;
        }

        Console.WriteLine("2) Please select your spice level:\n- High\n- Medium\n- Low");
        string spiceLevel = Console.ReadLine().Trim();

        Console.WriteLine("3) What do you prefer most?\n- North Indian\n- South Indian\n- Other");
        string cuisinePreference = Console.ReadLine().Trim();

        Console.WriteLine("4) Do you have a sweet tooth?\n- Yes\n- No");
        string sweetToothResponse = Console.ReadLine().Trim();
        bool sweetTooth = sweetToothResponse.Equals("Yes", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(Program.CurrentUsername))
        {
            Console.WriteLine("Error: No user is currently logged in.");
            return;
        }

        // Send the command to the server with properly formatted inputs
        string request = $"UPDATE_PROFILE {Program.CurrentUsername} \"{preference}\" \"{spiceLevel}\" \"{cuisinePreference}\" {sweetTooth.ToString().ToLower()}";
        string response = ServerCommunicator.SendCommandToServer(request);

        Console.WriteLine(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating profile: {ex.Message}");
    }
}


    }
}
