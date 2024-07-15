using System;
using CafeteriaClient.Services;

namespace CafeteriaClient.Authentications
{
    public static class Authentication
    {
        private static ServerCommunicator serverCommunicator = new ServerCommunicator();

        public static string Login(string username, string password)
        {
            if (IsInputInvalid(username, password))
            {
                return "Invalid login credentials. Username and password must not be empty.";
            }

            string command = $"LOGIN {username} {password}";
            Console.WriteLine($"Sending login command for user: {username}");

            try
            {
                return serverCommunicator.SendCommandToServer(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login attempt for user {username}: {ex.Message}");
                return $"Error communicating with server: {ex.Message}";
            }
        }

        private static bool IsInputInvalid(string username, string password)
        {
            return string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);
        }
    }
}
