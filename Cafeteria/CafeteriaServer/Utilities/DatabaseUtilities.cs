using MySql.Data.MySqlClient;

namespace CafeteriaServer.Utilities
{
    public static class DatabaseUtilities
    {
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
