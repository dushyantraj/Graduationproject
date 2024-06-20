
// using System;
// using CafeteriaClient.Communication;

// namespace CafeteriaClient.Authentications
// {
//     public static class Authentication
//     {
//         public static string Login(string username, string password)
//         {
//             string command = $"LOGIN {username} {password}";
//             return ServerCommunicator.SendCommandToServer(command);
//         }
//     }
// }
using System;
using CafeteriaClient.Communication;

namespace CafeteriaClient.Authentications
{
    public static class Authentication
    {
        public static string Login(string username, string password)
        {
            // Basic validation of input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Invalid login credentials. Username and password must not be empty.";
            }

            // Construct the login command
            string command = $"LOGIN {username} {password}";
            Console.WriteLine($"Sending login command for user: {username}");

            try
            {
                // Send command to the server and receive response
                string response = ServerCommunicator.SendCommandToServer(command);

                // Log the server response
                Console.WriteLine($"Server response for login attempt: {response}");

                return response;
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error during login attempt for user {username}: {ex.Message}");
                return $"Error communicating with server: {ex.Message}";
            }
        }
    }
}
