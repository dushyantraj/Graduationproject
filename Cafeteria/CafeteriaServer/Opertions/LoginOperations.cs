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
    }
}
