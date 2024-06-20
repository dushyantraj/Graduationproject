using MySql.Data.MySqlClient;
using System;
using System.Text;
using CafeteriaServer.Models;
namespace CafeteriaServer.Operations
{
    public static class EmployeeSelectionOperations
    {
       public static string FetchEmployeeSelections(MySqlConnection connection)
{
    try
    {
        StringBuilder sb = new StringBuilder();
        DateTime today = DateTime.Today;
        string todayString = today.ToString("yyyy-MM-dd");

        // Query to get today's employee selections and their corresponding item names
        string query = @"
            SELECT es.user_id, ri.rollout_id, ri.item_name 
            FROM EmployeeSelections es 
            JOIN RolloutItems ri ON es.rollout_id = ri.rollout_id 
            WHERE DATE(ri.date_rolled_out) = @today";

        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@today", todayString);

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                int userId = reader.GetInt32("user_id");
                int rolloutId = reader.GetInt32("rollout_id");
                string itemName = reader.GetString("item_name");

                sb.AppendLine($"Employee ID: {userId}, Rollout ID: {rolloutId}, Item Name: {itemName}");
            }
        }

        // If no records are found, provide a default message
        if (sb.Length == 0)
        {
            return "No employee selections found for today.";
        }

        return sb.ToString();
    }
    catch (Exception ex)
    {
        return "Error fetching employee selections: " + ex.Message;
    }
}

        public static string GetEmployeeNotification(int userTypeId, MySqlConnection connection)
{
try
    {
        StringBuilder sb = new StringBuilder();
        DateTime today = DateTime.Today;
        string todayString = today.ToString("yyyy-MM-dd");

        // Query to get today's rollout-related notifications for the specified userType_id
        string query = "SELECT message, notificationDateTime " +
                       "FROM Notifications " +
                       "WHERE userType_id = @userType_id " +
                       "AND DATE(notificationDateTime) = @today " +
                       "AND message LIKE '%rolled out%' " +
                       "ORDER BY notificationDateTime DESC";

        MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userType_id", userTypeId);
        command.Parameters.AddWithValue("@today", todayString);

        using (MySqlDataReader reader = command.ExecuteReader())
        {
            // Read all relevant notifications
            while (reader.Read())
            {
                string message = reader["message"].ToString();
                DateTime date = Convert.ToDateTime(reader["notificationDateTime"]);

                // Append each notification to the StringBuilder
                sb.AppendLine($"Notification: {message}, Date: {date}");
            }
        }

        // If no notifications are found, return a default message
        if (sb.Length == 0)
        {
            return "No rollout notifications found for today.";
        }

        return sb.ToString();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error fetching notifications: " + ex.Message);
        return null;
    }
}
   
    }
    
}
