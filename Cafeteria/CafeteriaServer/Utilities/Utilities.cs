using MySql.Data.MySqlClient;
using System;

namespace CafeteriaServer.Utilities
{
    public static class RelatedUtilites
    {
        public static string GetRoleName(MySqlConnection connection, int roleId)
        {
            return ExecuteScalarQuery(connection, "SELECT RoleName FROM Roles WHERE RoleID = @roleId",
                                      new MySqlParameter("@roleId", roleId))?.ToString()
                                      ?? $"Role with ID {roleId} not found";
        }

        public static int GetMenuId(MySqlConnection connection, string menuType)
        {
            object result = ExecuteScalarQuery(connection, "SELECT menu_id FROM Menu WHERE menu_type = @menuType",
                                               new MySqlParameter("@menuType", menuType));

            return result != null ? Convert.ToInt32(result) : -1;
        }

        private static object ExecuteScalarQuery(MySqlConnection connection, string query, params MySqlParameter[] parameters)
        {
            try
            {
                using (var cmd = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Log the exception message (logging implementation is not shown here)
                Console.WriteLine($"Error executing query: {ex.Message}");
                return null;
            }
        }
    }
}
