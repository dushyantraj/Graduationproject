using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using System;

namespace CafeteriaServer.Services
{
    public class UserProfileService
    {
        private readonly MySqlConnection _connection;

        public UserProfileService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public UserProfile FetchUserProfile(int userId)
        {
            string query = @"
                SELECT ProfileID, UserID, Preference, SpiceLevel, CuisinePreference, SweetTooth
                FROM EmployeeProfiles
                WHERE UserID = @userId";

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapUserProfile(reader);
                    }
                }
            }

            return null;
        }

        private UserProfile MapUserProfile(MySqlDataReader reader)
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
