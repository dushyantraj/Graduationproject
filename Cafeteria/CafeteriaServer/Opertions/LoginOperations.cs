using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Utilities;
using CafeteriaServer.Models;

public static class LoginOperations
{
    private static readonly HashSet<string> ValidRoles = new HashSet<string> { "Admin", "Chef", "Employee" };
    private const string LoginEventType = "LOGIN";
    private const string LogoutEventType = "LOGOUT";

    public static string LoginUser(MySqlConnection connection, UserCredentials credentials)
    {
        var userRepository = new UserRepository(connection);

        try
        {
            EnsureConnectionOpen(connection);

            int roleId = userRepository.GetRoleId(credentials);

            if (roleId == -1)
            {
                return "LOGIN_FAILURE: Invalid credentials.";
            }

            string roleName = DatabaseUtilities.GetRoleNameById(connection, roleId);

            if (!IsValidRole(roleName))
            {
                return "LOGIN_FAILURE: Invalid role.";
            }

            userRepository.LogUserActivity(credentials.Username, LoginEventType);
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
        var userRepository = new UserRepository(connection);

        try
        {
            EnsureConnectionOpen(connection);

            userRepository.LogUserActivity(username, LogoutEventType);
            return "LOGOUT_SUCCESS";
        }
        catch (Exception ex)
        {
            LogException($"Error during logout for user {username}", ex);
            return $"Error during logout for user {username}: {ex.Message}";
        }
    }

    private static bool IsValidRole(string roleName)
    {
        return ValidRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
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
