using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace CafeteriaServer.Operations
{
    public static class RolloutOperations
    {
        public static string GetRolloutItems(MySqlConnection connection, string rolloutType)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string query = "SELECT name, price FROM Rollout WHERE type = @type";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@type", rolloutType);

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    decimal price = Convert.ToDecimal(reader["price"]);

                    sb.AppendLine($"Name: {name}, Price: {price}");
                }

                reader.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching rollout items: {ex.Message}";
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
