using MySql.Data.MySqlClient;
using System;
using System.Text;
namespace CafeteriaServer.Operations
{
    public static class EmployeeSelectionOperations
    {
        public static string GetAvailableEmployees(MySqlConnection connection)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string query = "SELECT Name FROM Employee WHERE Available = 1";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    sb.AppendLine(name);
                }

                reader.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching available employees: {ex.Message}";
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
