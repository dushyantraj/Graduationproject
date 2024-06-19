using MySql.Data.MySqlClient;
using System;
using System.Text;
namespace CafeteriaServer.Operations
{
    public static class EmployeeSelectionOperations
    {
        public static string FetchEmployeeSelections(MySqlConnection connection)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string query = "SELECT es.*, ri.item_name FROM EmployeeSelections es JOIN RolloutItems ri ON es.rollout_id = ri.rollout_id";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int userId = reader.GetInt32("user_id");
                    int rolloutId = reader.GetInt32("rollout_id");
                    string itemName = reader.GetString("item_name");

                    sb.AppendLine($"Employee ID: {userId}, Rollout ID: {rolloutId}, Item Name: {itemName}");
                }

                reader.Close();

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "Error fetching employee selections: " + ex.Message;
            }
        }

    }
}
