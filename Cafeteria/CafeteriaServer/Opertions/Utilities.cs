using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CafeteriaServer
{
    public static class Utilities
    {
        public static string GetRoleName(MySqlConnection connection, int roleId)
        {
            try
            {
                string query = "SELECT RoleName FROM Roles WHERE RoleID = @roleId";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@roleId", roleId);

                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching role name: {ex.Message}";
            }
        }

        public static int GetMenuId(MySqlConnection connection, string menuType)
        {
               try
            {
                string query = $"SELECT menu_id FROM Menu WHERE menu_type = @menuType";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@menuType", menuType);

                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return -1; 
            }
            catch (Exception)
            {
                return -1;
            }
        }
        
    }
}
