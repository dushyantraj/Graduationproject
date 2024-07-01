using MySql.Data.MySqlClient;
using System;   
using CafeteriaServer.Utilities;
// CafeteriaServer/Repositories/EmployeeProfileRepository.cs
namespace CafeteriaServer.Repositories
{
    public class EmployeeProfileRepository
    {
        public bool ProfileExists(MySqlConnection connection, int userId)
        {
            string checkQuery = "SELECT COUNT(*) FROM EmployeeProfiles WHERE UserID = @userId";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@userId", userId);
                long profileCount = (long)checkCmd.ExecuteScalar();
                return profileCount > 0;
            }
        }

        public string UpdateProfile(MySqlConnection connection, int userId, string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
        {
            string updateQuery = @"
                UPDATE EmployeeProfiles 
                SET Preference = @preference, 
                    SpiceLevel = @spiceLevel, 
                    CuisinePreference = @cuisinePreference, 
                    SweetTooth = @sweetTooth 
                WHERE UserID = @userId";

            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
            {
                DatabaseUtilities.AddProfileParameters(updateCmd, userId, preference, spiceLevel, cuisinePreference, sweetTooth);
                int rowsAffected = updateCmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Profile updated successfully." : "Profile update failed.";
            }
        }

        public string InsertProfile(MySqlConnection connection, int userId, string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
        {
            string insertQuery = @"
                INSERT INTO EmployeeProfiles (UserID, Preference, SpiceLevel, CuisinePreference, SweetTooth) 
                VALUES (@userId, @preference, @spiceLevel, @cuisinePreference, @sweetTooth)";

            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
            {
                DatabaseUtilities.AddProfileParameters(insertCmd, userId, preference, spiceLevel, cuisinePreference, sweetTooth);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Profile created successfully." : "Profile creation failed.";
            }
        }

        public int GetUserIdByUsername(MySqlConnection connection, string username)
        {
            try
            {
                string query = "SELECT UserID FROM Users WHERE Username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching UserID: {ex.Message}");
                return -1;
            }
        }
    }
}
