using MySql.Data.MySqlClient;
using System;
using CafeteriaServer.Utilities;
using CafeteriaServer.Repositories;


namespace CafeteriaServer.Operations
{
    public class EmployeeProfileOperations
    {
        private readonly EmployeeProfileValidator _validator;
        private readonly EmployeeProfileRepository _repository;

        public EmployeeProfileOperations()
        {
            _validator = new EmployeeProfileValidator();
            _repository = new EmployeeProfileRepository();
        }

        public string UpdateOrCreateProfile(MySqlConnection connection, int userId, string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
        {
            try
            {
                Console.WriteLine($"Updating/Creating profile for userId={userId}, preference='{preference}', spiceLevel='{spiceLevel}', cuisinePreference='{cuisinePreference}', sweetTooth={sweetTooth}");

                if (!_validator.IsValidPreference(preference))
                {
                    return $"Invalid value for Preference: '{preference}'.";
                }

                if (_repository.ProfileExists(connection, userId))
                {
                    return _repository.UpdateProfile(connection, userId, preference, spiceLevel, cuisinePreference, sweetTooth);
                }
                else
                {
                    return _repository.InsertProfile(connection, userId, preference, spiceLevel, cuisinePreference, sweetTooth);
                }
            }
            catch (MySqlException ex)
            {
                return HandleMySqlException(ex, preference);
            }
            catch (Exception ex)
            {
                return $"Error updating profile: {ex.Message}";
            }
        }

        private string HandleMySqlException(MySqlException ex, string preference)
        {
            if (ex.Number == 1265)
            {
                return $"Error updating profile: Invalid value for Preference: '{preference}'.";
            }
            return $"Error updating profile: {ex.Message}";
        }
    }
}
