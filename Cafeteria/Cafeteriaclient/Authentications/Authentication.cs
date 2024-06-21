
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
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Invalid login credentials. Username and password must not be empty.";
            }

            string command = $"LOGIN {username} {password}";
            Console.WriteLine($"Sending login command for user: {username}");

            try
            {
                string response = ServerCommunicator.SendCommandToServer(command);

                Console.WriteLine($"Server response for login attempt: {response}");

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login attempt for user {username}: {ex.Message}");
                return $"Error communicating with server: {ex.Message}";
            }
        }
    }
}
