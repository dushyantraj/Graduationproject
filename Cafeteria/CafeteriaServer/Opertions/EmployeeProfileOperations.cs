using MySql.Data.MySqlClient;
using System;
using CafeteriaServer.Utilities;
using CafeteriaServer.Repositories;
using CafeteriaServer.Models;
namespace CafeteriaServer.Operations
{
    public class EmployeeProfileOperations
    {
        private readonly EmployeeProfileValidator _profileValidator;
        private readonly EmployeeProfileRepository _profileRepository;

        public EmployeeProfileOperations()
        {
            _profileValidator = new EmployeeProfileValidator();
            _profileRepository = new EmployeeProfileRepository();
        }

        public string SaveProfile(MySqlConnection connection, UserProfile profileData)
        {
            try
            {
                ValidateProfileData(profileData);

                if (_profileRepository.ProfileExists(connection, profileData.UserID))
                {
                    return _profileRepository.UpdateProfile(connection, profileData);
                }
                else
                {
                    return _profileRepository.InsertProfile(connection, profileData);
                }
            }
            catch (MySqlException ex)
            {
                return HandleDatabaseException(ex, profileData.Preference);
            }
            catch (Exception ex)
            {
                return $"Error saving profile: {ex.Message}";
            }
        }

        private void ValidateProfileData(UserProfile profileData)
        {
            if (!_profileValidator.IsValidPreference(profileData.Preference))
            {
                throw new ArgumentException($"Invalid value for Preference: '{profileData.Preference}'.");
            }
        }

        private string HandleDatabaseException(MySqlException ex, string preference)
        {
            if (ex.Number == 1265)
            {
                return $"Database error: Invalid value for Preference: '{preference}'.";
            }
            return $"Database error: {ex.Message}";
        }
    }
    public class ProfileData
    {
        public int UserId { get; set; }
        public string Preference { get; set; }
        public string SpiceLevel { get; set; }
        public string CuisinePreference { get; set; }
        public bool SweetTooth { get; set; }
    }
}
