using System;

namespace CafeteriaClient.Utilities
{
    class Utils
    {
        public static void Logout()
        {
            Console.WriteLine("Logging out...");
            Program.currentUsername = null;
            Program.currentRole = null;
        }

        // Additional utility methods can be added here as needed
    }
}
