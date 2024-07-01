// using System;

// namespace CafeteriaClient.Utilities
// {
//     class Utils
//     {
//         public static void Logout()
//         {
//             Console.WriteLine("Logging out...");
//             Program.currentUsername = null;
//             Program.currentRole = null;
//         }
//     }
// }
using System;
using CafeteriaClient.Communication;

namespace CafeteriaClient.Utilities
{
    public static class Utils
    {
        public static void Logout(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("No user is currently logged in.");
                return;
            }

            try
            {
                PerformLogout(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout for user {username}: {ex.Message}");
            }
        }

        private static void PerformLogout(string username)
        {
            string logoutCommand = $"LOGOUT {username}";
            string serverResponse = ServerCommunicator.SendCommandToServer(logoutCommand);

            if (serverResponse.StartsWith("LOGOUT_SUCCESS"))
            {
                Console.WriteLine("Logout successful.");
            }
            else
            {
                Console.WriteLine("Logout failed: " + serverResponse);
            }
        }
    }
}
