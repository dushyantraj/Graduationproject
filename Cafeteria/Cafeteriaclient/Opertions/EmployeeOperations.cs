using System;
using CafeteriaClient.Services;
using CafeteriaClient.Utilities;

namespace CafeteriaClient.Operations
{
    public static class EmployeeOperations
    {
        public static void FetchNotificationForEmployee()
        {
            ExecuteServerCommand(ServerCommands.FetchNotificationForEmployee, "notifications for employee");
        }

        public static void FetchEmployeeNotifications()
        {
            ExecuteServerCommand("FETCH_NOTIFICATIONS", "notifications");
        }

        public static void ViewEmployeeSelections()
        {
            ExecuteServerCommand(ServerCommands.FetchEmployeeSelections, "employee selections");
        }

        public static void UpdateProfile()
        {
            try
            {
                var profileUpdater = new ProfileUpdater();
                var profileDetails = profileUpdater.GetProfileDetails();

                if (profileDetails == null)
                {
                    // Error message already handled in GetProfileDetails
                    return;
                }

                string request = BuildUpdateProfileRequest(profileDetails);
                // Use a local instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string response = serverCommunicator.SendCommandToServer(request);

                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
            }
        }

        private static void ExecuteServerCommand(string command, string description)
        {
            try
            {
                // Use a local instance of ServerCommunicator
                var serverCommunicator = new ServerCommunicator();
                string response = serverCommunicator.SendCommandToServer(command);
                Console.WriteLine($"Received from server ({description}):\n{response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching {description}: {ex.Message}");
            }
        }

        private static string BuildUpdateProfileRequest(ProfileDetails profile)
        {
            return $"{ServerCommands.UpdateProfile} {Program.CurrentUsername} \"{profile.Preference}\" \"{profile.SpiceLevel}\" \"{profile.CuisinePreference}\" {profile.SweetTooth.ToString().ToLower()}";
        }
    }

    // Helper class to handle profile updates
    public class ProfileUpdater
    {
        public ProfileDetails GetProfileDetails()
        {
            if (string.IsNullOrEmpty(Program.CurrentUsername))
            {
                Console.WriteLine("Error: No user is currently logged in.");
                return null;
            }

            Console.WriteLine("Please answer these questions to update your profile:");

            string preference = GetValidatedPreference();
            if (preference == null) return null;

            string spiceLevel = GetSpiceLevel();
            string cuisinePreference = GetCuisinePreference();
            bool sweetTooth = GetSweetToothPreference();

            return new ProfileDetails(preference, spiceLevel, cuisinePreference, sweetTooth);
        }

        private string GetValidatedPreference()
        {
            Console.WriteLine("1) Please select one:\n- Vegetarian\n- Non Vegetarian\n- Eggetarian");
            string preference = Console.ReadLine().Trim();

            string[] validPreferences = { "Vegetarian", "Non Vegetarian", "Eggetarian" };
            if (!Array.Exists(validPreferences, p => p.Equals(preference, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Error: Invalid value for Preference: '{preference}'.");
                return null;
            }

            return preference;
        }

        private string GetSpiceLevel()
        {
            Console.WriteLine("2) Please select your spice level:\n- High\n- Medium\n- Low");
            return Console.ReadLine().Trim();
        }

        private string GetCuisinePreference()
        {
            Console.WriteLine("3) What do you prefer most?\n- North Indian\n- South Indian\n- Other");
            return Console.ReadLine().Trim();
        }

        private bool GetSweetToothPreference()
        {
            Console.WriteLine("4) Do you have a sweet tooth?\n- Yes\n- No");
            string sweetToothResponse = Console.ReadLine().Trim();
            return sweetToothResponse.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        }
    }

    // Model class to encapsulate profile details
    public class ProfileDetails
    {
        public string Preference { get; }
        public string SpiceLevel { get; }
        public string CuisinePreference { get; }
        public bool SweetTooth { get; }

        public ProfileDetails(string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
        {
            Preference = preference;
            SpiceLevel = spiceLevel;
            CuisinePreference = cuisinePreference;
            SweetTooth = sweetTooth;
        }
    }
}
