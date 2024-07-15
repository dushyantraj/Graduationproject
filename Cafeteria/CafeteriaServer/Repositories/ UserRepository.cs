using MySql.Data.MySqlClient;
using System;
using CafeteriaServer.Models;

public class UserRepository
{
    private readonly MySqlConnection _connection;

    public UserRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public UserProfile GetUserProfile(int userId)
    {
        string query = @"
            SELECT ProfileID, UserID, Preference, SpiceLevel, CuisinePreference, SweetTooth
            FROM EmployeeProfiles
            WHERE UserID = @userId";

        using (MySqlCommand cmd = new MySqlCommand(query, _connection))
        {
            cmd.Parameters.AddWithValue("@userId", userId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return MapUserProfile(reader);
                }
            }
        }

        return null;
    }

    private UserProfile MapUserProfile(MySqlDataReader reader)
    {
        return new UserProfile
        {
            ProfileID = reader.GetInt32("ProfileID"),
            UserID = reader.GetInt32("UserID"),
            Preference = reader.GetString("Preference"),
            SpiceLevel = reader.GetString("SpiceLevel"),
            CuisinePreference = reader.GetString("CuisinePreference"),
            SweetTooth = reader.GetBoolean("SweetTooth")
        };
    }

    public int GetRoleId(UserCredentials credentials)
    {
        string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";

        using (MySqlCommand cmd = new MySqlCommand(query, _connection))
        {
            cmd.Parameters.AddWithValue("@username", credentials.Username);
            cmd.Parameters.AddWithValue("@password", credentials.Password);

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }
    }

    public void LogUserActivity(string username, string eventType)
    {
        string logQuery = "INSERT INTO UserActivityLog (Username, EventType, EventTime) VALUES (@username, @eventType, @eventTime)";

        using (MySqlCommand logCmd = new MySqlCommand(logQuery, _connection))
        {
            logCmd.Parameters.AddWithValue("@username", username);
            logCmd.Parameters.AddWithValue("@eventType", eventType);
            logCmd.Parameters.AddWithValue("@eventTime", DateTime.UtcNow);

            logCmd.ExecuteNonQuery();
            Console.WriteLine($"Activity logged: {username}, {eventType}");
        }
    }
}
