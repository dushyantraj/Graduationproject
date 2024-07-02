// using MySql.Data.MySqlClient;
// using System;
// using CafeteriaServer.Utilities;

// public static class LoginOperations
// {
//     public static string LoginUser(MySqlConnection connection, string username, string password)
//     {
//         try
//         {
//             EnsureConnectionOpen(connection);

//             int roleId = GetRoleId(connection, username, password);

//             if (roleId != -1)
//             {
//                 string roleName = RelatedUtilites.GetRoleName(connection, roleId);

//                 if (IsValidRole(roleName))
//                 {
//                     LogUserActivity(connection, username, "LOGIN");
//                     return $"LOGIN_SUCCESS {roleName}";
//                 }
//                 else
//                 {
//                     return "LOGIN_FAILURE: Invalid role.";
//                 }
//             }
//             else
//             {
//                 return "LOGIN_FAILURE: Invalid credentials.";
//             }
//         }
//         catch (Exception ex)
//         {
//             LogException("Error during login", ex);
//             return $"Error during login: {ex.Message}";
//         }
//     }

//     public static string LogoutUser(MySqlConnection connection, string username)
//     {
//         try
//         {
//             EnsureConnectionOpen(connection);

//             LogUserActivity(connection, username, "LOGOUT");
//             return "LOGOUT_SUCCESS";
//         }
//         catch (Exception ex)
//         {
//             LogException($"Error during logout for user {username}", ex);
//             return $"Error during logout for user {username}: {ex.Message}";
//         }
//     }

//     private static int GetRoleId(MySqlConnection connection, string username, string password)
//     {
//         string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";

//         using (MySqlCommand cmd = new MySqlCommand(query, connection))
//         {
//             cmd.Parameters.AddWithValue("@username", username);
//             cmd.Parameters.AddWithValue("@password", password);

//             object result = cmd.ExecuteScalar();

//             return result != null ? Convert.ToInt32(result) : -1;
//         }
//     }

//     private static bool IsValidRole(string roleName)
//     {
//         var validRoles = new HashSet<string> { "Admin", "Chef", "Employee" };
//         return validRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
//     }

//     private static void LogUserActivity(MySqlConnection connection, string username, string eventType)
//     {
//         string logQuery = "INSERT INTO UserActivityLog (Username, EventType, EventTime) VALUES (@username, @eventType, @eventTime)";

//         using (MySqlCommand logCmd = new MySqlCommand(logQuery, connection))
//         {
//             logCmd.Parameters.AddWithValue("@username", username);
//             logCmd.Parameters.AddWithValue("@eventType", eventType);
//             logCmd.Parameters.AddWithValue("@eventTime", DateTime.UtcNow);

//             logCmd.ExecuteNonQuery();
//             Console.WriteLine($"Activity logged: {username}, {eventType}");
//         }
//     }

//     private static void LogException(string message, Exception ex)
//     {
//         Console.WriteLine($"{message}: {ex.Message}");
//     }

//     private static void EnsureConnectionOpen(MySqlConnection connection)
//     {
//         if (connection.State == System.Data.ConnectionState.Closed)
//         {
//             connection.Open();
//         }
//     }
// }

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Utilities;

public static class LoginOperations
{
    private static readonly HashSet<string> ValidRoles = new HashSet<string> { "Admin", "Chef", "Employee" };
    private const string LoginEventType = "LOGIN";
    private const string LogoutEventType = "LOGOUT";

    public static string LoginUser(MySqlConnection connection, string username, string password)
    {
        try
        {
            EnsureConnectionOpen(connection);

            int roleId = GetRoleId(connection, username, password);

            if (roleId == -1)
            {
                return "LOGIN_FAILURE: Invalid credentials.";
            }

            string roleName = DatabaseUtilities.GetRoleNameById(connection, roleId);

            if (!IsValidRole(roleName))
            {
                return "LOGIN_FAILURE: Invalid role.";
            }

            LogUserActivity(connection, username, LoginEventType);
            return $"LOGIN_SUCCESS {roleName}";
        }
        catch (Exception ex)
        {
            LogException("Error during login", ex);
            return $"Error during login: {ex.Message}";
        }
    }

    public static string LogoutUser(MySqlConnection connection, string username)
    {
        try
        {
            EnsureConnectionOpen(connection);

            LogUserActivity(connection, username, LogoutEventType);
            return "LOGOUT_SUCCESS";
        }
        catch (Exception ex)
        {
            LogException($"Error during logout for user {username}", ex);
            return $"Error during logout for user {username}: {ex.Message}";
        }
    }

    private static int GetRoleId(MySqlConnection connection, string username, string password)
    {
        string query = "SELECT RoleID FROM Users WHERE Username = @username AND Password = @password";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }
    }

    private static bool IsValidRole(string roleName)
    {
        return ValidRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    private static void LogUserActivity(MySqlConnection connection, string username, string eventType)
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

    private static void LogException(string message, Exception ex)
    {
        Console.WriteLine($"{message}: {ex.Message}");
    }

    private static void EnsureConnectionOpen(MySqlConnection connection)
    {
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
    }
}
