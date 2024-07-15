using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using CafeteriaServer.Models;

namespace CafeteriaServer.Repositories
{
    public class NotificationRepository
    {
        private readonly MySqlConnection _connection;

        public NotificationRepository(MySqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void AddNotification(Notification notification)
        {
            const string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
            ExecuteNonQuery(query, notification);
        }

        public Notification GetLatestChefNotification(int userTypeId)
        {
            const string query = "SELECT message, notificationDateTime " +
                                 "FROM Notifications " +
                                 "WHERE userType_id = @userType_id " +
                                 "ORDER BY notificationDateTime DESC " +
                                 "LIMIT 1";

            return GetNotification(query, userTypeId);
        }

        public List<Notification> GetRolloutNotificationsForToday(int userTypeId)
        {
            DateTime today = DateTime.Today;
            string todayString = today.ToString("yyyy-MM-dd");
            const string query = "SELECT message, notificationDateTime " +
                                    "FROM Notifications " +
                                    "WHERE userType_id = @userType_id " +
                                    "AND DATE(notificationDateTime) = @today " +
                                    "AND (message LIKE '%rolled out%' OR message Like '%New item added%' OR message = '') " +
                                    "ORDER BY notificationDateTime DESC";

            return GetNotifications(query, userTypeId, todayString);
        }

        public void NotifyEmployeesAndChef(string itemName)
        {
            AddNotification(new Notification
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 2
            });

            AddNotification(new Notification
            {
                Message = $"New item added: {itemName}",
                Date = DateTime.Now,
                Role = 3
            });
        }

        private void ExecuteNonQuery(string query, Notification notification)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@message", notification.Message);
                command.Parameters.AddWithValue("@userType_id", notification.Role);
                command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogException("Error executing query", ex);
            }
        }

        private Notification GetNotification(string query, int userTypeId)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@userType_id", userTypeId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Notification
                        {
                            Message = reader.GetString("message"),
                            Date = Convert.ToDateTime(reader["notificationDateTime"])
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LogException("Error fetching notification", ex);
                return null;
            }
        }

        private List<Notification> GetNotifications(string query, int userTypeId, string todayString)
        {
            try
            {
                List<Notification> notifications = new List<Notification>();

                MySqlCommand command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@userType_id", userTypeId);
                command.Parameters.AddWithValue("@today", todayString);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notifications.Add(new Notification
                        {
                            Message = reader.GetString("message"),
                            Date = Convert.ToDateTime(reader["notificationDateTime"])
                        });
                    }
                }

                return notifications;
            }
            catch (Exception ex)
            {
                LogException("Error fetching notifications", ex);
                return new List<Notification>();
            }
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
