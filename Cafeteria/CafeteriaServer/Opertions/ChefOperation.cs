using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data;
using CafeteriaServer.Recommendation;
using CafeteriaServer.Models;
namespace CafeteriaServer.Operations
{
    public static class ChefOperation
    {
        public static string RolloutFoodItemsForNextDay(MySqlConnection connection, string[] itemIds)
        {
            try
            {
                int successfulCount = 0;
                DateTime today = DateTime.Today;

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach (var itemIdStr in itemIds)
                    {
                        if (int.TryParse(itemIdStr, out int itemId))
                        {
                            string checkQuery = "SELECT COUNT(*) FROM RolloutItems WHERE item_id = @itemId AND date_rolled_out = @today";
                            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection, transaction);
                            checkCmd.Parameters.AddWithValue("@itemId", itemId);
                            checkCmd.Parameters.AddWithValue("@today", today);

                            long count = (long)checkCmd.ExecuteScalar();

                            if (count == 0)
                            {
                                string getItemQuery = "SELECT * FROM MenuItem WHERE item_id = @itemId";
                                MySqlCommand getItemCmd = new MySqlCommand(getItemQuery, connection, transaction);
                                getItemCmd.Parameters.AddWithValue("@itemId", itemId);

                                using (MySqlDataReader reader = getItemCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string itemName = reader.GetString("name");
                                        decimal price = reader.GetDecimal("price");
                                        int available = reader.GetInt32("available");

                                        reader.Close();

                                        string insertQuery = "INSERT INTO RolloutItems (item_id, item_name, price, available, selected_for_next_day, date_rolled_out) " +
                                                             "VALUES (@itemId, @itemName, @price, @available, true, @today)";
                                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction);
                                        insertCmd.Parameters.AddWithValue("@itemId", itemId);
                                        insertCmd.Parameters.AddWithValue("@itemName", itemName);
                                        insertCmd.Parameters.AddWithValue("@price", price);
                                        insertCmd.Parameters.AddWithValue("@available", available);
                                        insertCmd.Parameters.AddWithValue("@today", today);

                                        int rowsAffected = insertCmd.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            successfulCount++;
                                            string notificationMessage = $"Item '{itemName}' has been rolled out for the next day.";
                                            Notification notification = new Notification
                                            {
                                                Message = notificationMessage,
                                                Role = 3,
                                                Date = DateTime.Now
                                            };
                                            RolloutNotification(notification, connection);
                                        }
                                    }
                                    else
                                    {
                                        reader.Close();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Item with ID {itemId} has already been rolled out today.");
                            }
                        }
                    }
                    transaction.Commit();
                }

                if (successfulCount == itemIds.Length)
                {
                    return "Items rolled out for next day successfully.";
                }
                else if (successfulCount > 0)
                {
                    return "Some items were already rolled out today. Only new items were added.";
                }
                else
                {
                    return "All selected items were already rolled out today.";
                }
            }
            catch (Exception ex)
            {
                return "Error rolling out item: " + ex.Message;
            }
        }

        public static void RolloutNotification(Notification notification, MySqlConnection connection)
        {
            try
            {
                string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) VALUES (@message, @userType_id, @notificationDateTime)";
                MySqlCommand command = new MySqlCommand(query, connection);

                // Use the appropriate parameters matching the table columns
                command.Parameters.AddWithValue("@message", notification.Message);
                command.Parameters.AddWithValue("@userType_id", notification.Role);
                command.Parameters.AddWithValue("@notificationDateTime", notification.Date);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding notification: " + ex.Message);
            }
        }
        public static string GetChefNotification(int userTypeId, MySqlConnection connection)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                // Query to get the latest notification for the specified userType_id
                string query = "SELECT message, userType_id, notificationDateTime " +
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
                            Message = reader["message"].ToString(),
                            Date = Convert.ToDateTime(reader["notificationDateTime"])
                        };
                        sb.AppendLine($"Notification: {notification.Message}, Date: {notification.Date}");
                    }
                }

                // If no record is found, return null or you could return a default Notification object
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching notification: " + ex.Message);
                return null;
            }
        }

    }
}
