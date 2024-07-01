using System;
using CafeteriaClient.Services;

namespace CafeteriaClient.Operations
{
    public static class NotificationOperations
    {
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
    }
}
