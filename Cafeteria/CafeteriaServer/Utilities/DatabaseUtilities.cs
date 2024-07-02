using MySql.Data.MySqlClient;

namespace CafeteriaServer.Utilities
{
    public static class DatabaseUtilities
    {


        private const string RoleNotFoundMessage = "Role with ID {0} not found";
        private const int InvalidMenuId = -1;

        public static string GetRoleNameById(MySqlConnection dbConnection, int roleId)
        {
            string query = "SELECT RoleName FROM Roles WHERE RoleID = @roleId";
            var queryParameters = new MySqlParameter[] { new MySqlParameter("@roleId", roleId) };

            var roleName = ExecuteScalarQuery(dbConnection, query, queryParameters)?.ToString();
            return roleName ?? string.Format(RoleNotFoundMessage, roleId);
        }

        public static int GetMenuIdByType(MySqlConnection dbConnection, string menuType)
        {
            string query = "SELECT menu_id FROM Menu WHERE menu_type = @menuType";
            var queryParameters = new MySqlParameter[] { new MySqlParameter("@menuType", menuType) };

            object result = ExecuteScalarQuery(dbConnection, query, queryParameters);
            return result != null ? Convert.ToInt32(result) : InvalidMenuId;
        }

        private static object ExecuteScalarQuery(MySqlConnection dbConnection, string sqlQuery, params MySqlParameter[] sqlParameters)
        {
            try
            {
                using (var sqlCommand = new MySqlCommand(sqlQuery, dbConnection))
                {
                    if (sqlParameters != null)
                    {
                        sqlCommand.Parameters.AddRange(sqlParameters);
                    }

                    return sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception exception)
            {
                LogError("Error executing SQL query", exception);
                return null;
            }
        }

        private static void LogError(string errorMessage, Exception exception)
        {
            Console.WriteLine($"{errorMessage}: {exception.Message}");
        }
        public static void AddProfileParameters(MySqlCommand cmd, int userId, string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
        {
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@preference", preference);
            cmd.Parameters.AddWithValue("@spiceLevel", spiceLevel);
            cmd.Parameters.AddWithValue("@cuisinePreference", cuisinePreference);
            cmd.Parameters.AddWithValue("@sweetTooth", sweetTooth);
        }
    }
}
