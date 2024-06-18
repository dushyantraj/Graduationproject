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
                string query = "SELECT menu_id FROM Menu WHERE type = @menuType";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@menuType", menuType);

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> feedbackEntries)
        {
            double averageRating = 0;
            int positiveCount = 0;
            int negativeCount = 0;

            foreach (var entry in feedbackEntries)
            {
                averageRating += entry.Rating;
                if (entry.Rating >= 3.0)
                {
                    positiveCount++;
                }
                else
                {
                    negativeCount++;
                }
            }

            averageRating /= feedbackEntries.Count;

            string overallSentiment = positiveCount >= negativeCount ? "Positive" : "Negative";
            string recommendation = averageRating >= 3.0 ? "Highly Recommended" : "Not Recommended";

            return (averageRating, overallSentiment, recommendation);
        }
    }
}
