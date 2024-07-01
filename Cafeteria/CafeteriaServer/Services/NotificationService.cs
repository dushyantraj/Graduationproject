
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using System.Text;

namespace CafeteriaServer.Services
{
    public class NotificationService
    {
        private readonly MySqlConnection _connection;

        public NotificationService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public void SendNotification(Notification notification)
        {
            const string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
            MySqlCommand command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@message", notification.Message);
            command.Parameters.AddWithValue("@userType_id", notification.Role);
            command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

            command.ExecuteNonQuery();
        }

        public static void RolloutNotification(Notification notification, MySqlConnection connection)
        {
            const string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@message", notification.Message);
            command.Parameters.AddWithValue("@userType_id", notification.Role);
            command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

            command.ExecuteNonQuery();
        }

        public static string GetChefNotification(int userTypeId, MySqlConnection connection)
        {
            try
            {
                const string query = "SELECT message, notificationDateTime " +
                                     "FROM Notifications " +
                                     "WHERE userType_id = @userType_id " +
                                     "ORDER BY notificationDateTime DESC " +
                                     "LIMIT 1";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userType_id", userTypeId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Notification notification = new Notification
                        {
                            Message = reader.GetString("message"),
                            Date = Convert.ToDateTime(reader["notificationDateTime"])
                        };
                        return $"Notification: {notification.Message}, Date: {notification.Date}";
                    }
                }

                return "No notifications found.";
            }
            catch (Exception ex)
            {
                LogException("Error fetching notification", ex);
                return "Error fetching notification: " + ex.Message;
            }
        }

        public static string GetEmployeeNotification(int userTypeId, MySqlConnection connection)
        {
            try
            {
                DateTime today = DateTime.Today;
                string todayString = today.ToString("yyyy-MM-dd");

                const string query = "SELECT message, notificationDateTime " +
                                     "FROM Notifications " +
                                     "WHERE userType_id = @userType_id " +
                                     "AND DATE(notificationDateTime) = @today " +
                                     "AND message LIKE '%rolled out%' " +
                                     "ORDER BY notificationDateTime DESC";

                StringBuilder sb = new StringBuilder();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userType_id", userTypeId);
                    command.Parameters.AddWithValue("@today", todayString);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string message = reader["message"].ToString();
                            DateTime date = Convert.ToDateTime(reader["notificationDateTime"]);

                            sb.AppendLine($"Notification: {message}, Date: {date}");
                        }
                    }
                }

                return sb.Length > 0 ? sb.ToString() : "No rollout notifications found for today.";
            }
            catch (Exception ex)
            {
                LogException("Error fetching notifications", ex);
                return "Error fetching notifications: " + ex.Message;
            }
        }

        public static void NotifyEmployeesAndChef(MySqlConnection connection, string itemName)
        {
            AddNotification(new Notification
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 2
            }, connection);

            AddNotification(new Notification
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 3
            }, connection);
        }
        public static void AddNotification(Notification notification, MySqlConnection connection)
        {
            const string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
            MySqlCommand command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@message", notification.Message);
            command.Parameters.AddWithValue("@userType_id", notification.Role);
            command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

            command.ExecuteNonQuery();
        }

        private static void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
