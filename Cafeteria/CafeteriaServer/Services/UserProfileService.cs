using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using System;

namespace CafeteriaServer.Services
{
    public class UserProfileService
    {
        public UserProfile FetchUserProfile(MySqlConnection connection, int userId)
        {
            string query = @"
                SELECT ProfileID, UserID, Preference, SpiceLevel, CuisinePreference, SweetTooth
                FROM EmployeeProfiles
                WHERE UserID = @userId";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
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
                }
            }

            return null;
        }
    }
}
