using System;
using System.Text;
using MySql.Data.MySqlClient;

namespace CafeteriaServer.Services
{
    public class EmployeeSelectionService
    {
        private readonly MySqlConnection _connection;

        public EmployeeSelectionService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public string FetchEmployeeSelections()
        {
            try
            {
                DateTime today = DateTime.Today;
                string todayString = today.ToString("yyyy-MM-dd");

                string query = @"
                    SELECT es.user_id, ri.rollout_id, ri.item_name 
                    FROM EmployeeSelections es 
                    JOIN RolloutItems ri ON es.rollout_id = ri.rollout_id 
                    WHERE DATE(ri.date_rolled_out) = @today";

                StringBuilder sb = new StringBuilder();

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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
                }

                if (sb.Length == 0)
                {
                    return "No employee selections found for today.";
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                LogException("Error fetching employee selections", ex);
                return "Error fetching employee selections: " + ex.Message;
            }
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
