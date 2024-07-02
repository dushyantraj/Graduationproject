
using System;
using CafeteriaClient.Services;
using CafeteriaClient.Utilities;
namespace CafeteriaClient.Operations
{
    public static class EmployeeOperations
    {
        public static void FetchNotificationForEmployee()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer(ServerCommands.FetchNotificationForEmployee);
                Console.WriteLine("Received from server:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications for employee: {ex.Message}");
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
        public static void ViewEmployeeSelections()
        {
            try
            {
                string response = ServerCommunicator.SendCommandToServer(ServerCommands.FetchEmployeeSelections);
                Console.WriteLine("Employee Selections:\n{0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching employee selections: {ex.Message}");
            }
        }

        public static void UpdateProfile()
        {
            try
            {
                Console.WriteLine("Please answer these questions to update your profile:");

                Console.WriteLine("1) Please select one:\n- Vegetarian\n- Non Vegetarian\n- Eggetarian");
                string preference = Console.ReadLine().Trim();

                // Ensure exact match for ENUM values
                string[] validPreferences = { "Vegetarian", "Non Vegetarian", "Eggetarian" };
                if (!Array.Exists(validPreferences, p => p.Equals(preference, StringComparison.OrdinalIgnoreCase)))
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

                string request = $"{ServerCommands.UpdateProfile} {Program.CurrentUsername} \"{preference}\" \"{spiceLevel}\" \"{cuisinePreference}\" {sweetTooth.ToString().ToLower()}";
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


