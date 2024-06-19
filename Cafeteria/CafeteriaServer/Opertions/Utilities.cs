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
        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            var overallMetrics = CalculateOverallSentimentsAndRatings(entries);
            return overallMetrics;
        }

        public static (double AverageRating, string OverallSentiment, string Recommendation) CalculateOverallSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {

            Dictionary<string, List<string>> sentimentFeatures = new Dictionary<string, List<string>>
            {
                { "Positive", new List<string> { "delicious", "amazing", "great", "fantastic", "excellent", "good", "tasty", "wonderful", "superb", "awesome", "very good" } },
                { "Negative", new List<string> { "bad", "terrible", "disappointing", "awful", "poor", "tasteless", "horrible", "gross", "unpleasant", "mediocre", "not good", "bad taste" } }
            };

            int positiveCount = 0;
            int negativeCount = 0;
            int neutralCount = 0;

            double totalRating = 0;

            foreach (var entry in entries)
            {
                string comment = entry.Comment;
                int score = 0;


                foreach (var kvp in sentimentFeatures)
                {
                    foreach (var feature in kvp.Value)
                    {
                        if (comment.Contains(feature, StringComparison.OrdinalIgnoreCase))
                        {
                            score += kvp.Key == "Positive" ? 1 : -1;
                        }
                    }
                }
                if (score > 0)
                {
                    positiveCount++;
                }
                else if (score < 0)
                {
                    negativeCount++;
                }
                else
                {
                    neutralCount++;
                }


                totalRating += entry.Rating;
            }

            double averageRating = entries.Count > 0 ? totalRating / entries.Count : 0.0;

            string overallSentiment = "Neutral";
            if (positiveCount > negativeCount && positiveCount > neutralCount)
            {
                overallSentiment = "Positive";
            }
            else if (negativeCount > positiveCount && negativeCount > neutralCount)
            {
                overallSentiment = "Negative";
            }
            else if (positiveCount > 0 && negativeCount > 0)
            {
                overallSentiment = "Mixed";
            }

            string recommendation = averageRating >= 4.0 ? "Highly recommended" :
                                    averageRating >= 3.0 ? "Recommended" :
                                    averageRating >= 2.0 ? "Average" :
                                    "Not recommended";

            return (averageRating, overallSentiment, recommendation);
        }
    }
}
