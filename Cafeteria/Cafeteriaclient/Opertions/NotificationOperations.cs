using System;
using CafeteriaClient.Services;

namespace CafeteriaClient.Operations
{
    public static class NotificationOperations
    {
        private static ServerCommunicator serverCommunicator = new ServerCommunicator();

        public static void FetchEmployeeNotifications()
        {
            try
            {
                string response = serverCommunicator.SendCommandToServer("FETCH_NOTIFICATIONS");
                Console.WriteLine("Your Notifications:\n" + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching notifications: {ex.Message}");
            }
        }
    }
}
