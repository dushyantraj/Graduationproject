using MySql.Data.MySqlClient;
using System;


namespace CafeteriaServer.Operations
{
    public static class LoginOperations
    {
        public static string LoginUser(MySqlConnection connection, string username, string password)
        {
            try
            {
                string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int roleId = Convert.ToInt32(result);
                    string roleName = Utilities.GetRoleName(connection, roleId);

                    if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        roleName.Equals("Chef", StringComparison.OrdinalIgnoreCase) ||
                        roleName.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                    {
                        LogUserActivity(connection, username, "LOGIN");
                        return $"LOGIN_SUCCESS {roleName}";
                    }
                    else
                    {
                        return "LOGIN_FAILURE Invalid role.";
                    }
                }
                else
                {
                    return "LOGIN_FAILURE Invalid credentials.";
                }
            }
            catch (Exception ex)
            {
                return "Error during login: " + ex.Message;
            }
        }
        public static string LogoutUser(MySqlConnection connection, string username)
        {
            try
            {
                Console.WriteLine($"Logging out user: {username}");
                LogUserActivity(connection, username, "LOGOUT");
                Console.WriteLine($"Logout event logged for user: {username}");
                return "LOGOUT_SUCCESS";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout for user {username}: {ex.Message}");
                return "Error during logout: " + ex.Message;
            }
        }

        private static void LogUserActivity(MySqlConnection connection, string username, string eventType)
        {
            try
            {
                string logQuery = "INSERT INTO UserActivityLog (Username, EventType) VALUES (@username, @eventType)";
                MySqlCommand logCmd = new MySqlCommand(logQuery, connection);
                logCmd.Parameters.AddWithValue("@username", username);
                logCmd.Parameters.AddWithValue("@eventType", eventType);
                logCmd.ExecuteNonQuery();
                Console.WriteLine($"Activity logged: {username}, {eventType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging activity for user {username}: {ex.Message}");
            }
        }

    }
}
