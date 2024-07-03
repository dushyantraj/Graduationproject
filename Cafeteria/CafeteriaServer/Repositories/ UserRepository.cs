using MySql.Data.MySqlClient;
using System;
using CafeteriaServer.Models;
public class UserRepository
{
    public int GetRoleId(MySqlConnection connection, UserCredentials credentials)
    {
        string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@username", credentials.Username);
            cmd.Parameters.AddWithValue("@password", credentials.Password);

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }
    }

    public void LogUserActivity(MySqlConnection connection, string username, string eventType)
    {
        string logQuery = "INSERT INTO UserActivityLog (Username, EventType, EventTime) VALUES (@username, @eventType, @eventTime)";

        using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection))
        {
            logCmd.Parameters.AddWithValue("@username", username);
            logCmd.Parameters.AddWithValue("@eventType", eventType);
            logCmd.Parameters.AddWithValue("@eventTime", DateTime.UtcNow);

            logCmd.ExecuteNonQuery();
            Console.WriteLine($"Activity logged: {username}, {eventType}");
        }
    }
}
