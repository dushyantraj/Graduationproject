using System.Text;
using System;
using CafeteriaServer.Models;
using CafeteriaServer.Repositories;
using MySql.Data.MySqlClient;

namespace CafeteriaServer.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;

        public NotificationService(MySqlConnection connection)
        {
            _repository = new NotificationRepository(connection);
        }

        public void SendNotification(Notification notification)
        {
            _repository.AddNotification(notification);
        }
        public void SendemployeeNotification(Notification notification)
        {
            _repository.AddNotification(notification);
        }

        public string GetChefNotification(int userTypeId)
        {
            Notification latestNotification = _repository.GetLatestChefNotification(userTypeId);

            if (latestNotification != null)
            {
                return $"Notification: {latestNotification.Message}, Date: {latestNotification.Date}";
            }

            return "No notifications found.";
        }

        public string GetEmployeeNotification(int userTypeId)
        {
            var notifications = _repository.GetRolloutNotificationsForToday(userTypeId);

            if (notifications.Count > 0)
            {
                var response = new StringBuilder();
                response.AppendLine("Rollout Notifications for Today:");

                foreach (var notification in notifications)
                {
                    response.AppendLine($"Notification: {notification.Message}, Date: {notification.Date}");
                }

                return response.ToString();
            }

            return "No rollout notifications found for today.";
        }

        public void NotifyEmployeesAndChef(string itemName)
        {
            _repository.NotifyEmployeesAndChef(itemName);
        }
    }
}
