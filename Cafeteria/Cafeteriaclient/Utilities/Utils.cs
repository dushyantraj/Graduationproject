using System;
using CafeteriaClient.Services;

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
            try
            {
                // Assuming ServerCommunicator needs to be instantiated
                ServerCommunicator serverCommunicator = new ServerCommunicator();
                string logoutCommand = $"LOGOUT {username}";
                string serverResponse = serverCommunicator.SendCommandToServer(logoutCommand);

                if (serverResponse.StartsWith("LOGOUT_SUCCESS"))
                {
                    Console.WriteLine("Logout successful.");
                }
                else
                {
                    Console.WriteLine("Logout failed: " + serverResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with server during logout: {ex.Message}");
            }
        }
    }
}
