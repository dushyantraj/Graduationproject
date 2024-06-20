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
    class Utils
    {
        public static void Logout()
        {
            if (Program.currentUsername != null)
            {
                // Send LOGOUT command to the server
                string logoutCommand = $"LOGOUT {Program.currentUsername}";
                string serverResponse = ServerCommunicator.SendCommandToServer(logoutCommand);

                // Check the server's response
                if (serverResponse.StartsWith("LOGOUT_SUCCESS"))
                {
                    Console.WriteLine("Logout successful.");
                }
                else
                {
                    Console.WriteLine("Logout failed: " + serverResponse);
                }

                // Clear the current session
                Program.currentUsername = null;
                Program.currentRole = null;

                Console.WriteLine("You have been logged out.");
            }
            else
            {
                Console.WriteLine("No user is currently logged in.");
            }
        }
    }
}
