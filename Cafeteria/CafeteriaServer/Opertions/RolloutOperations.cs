using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Recommendation;
using CafeteriaServer.Models;
namespace CafeteriaServer.Operations
{
    public static class RolloutOperations
    {
        public static string FetchRolloutItemsWithFeedback(MySqlConnection connection)
        {
            try
            {
                DateTime today = DateTime.Today;
                string todayString = today.ToString("yyyy-MM-dd");
                string rolloutQuery = @"
            SELECT rollout_id, item_name, price, available 
            FROM RolloutItems 
            WHERE available = 1 AND DATE(date_rolled_out) = @today";

                MySqlCommand rolloutCmd = new MySqlCommand(rolloutQuery, connection);
                rolloutCmd.Parameters.AddWithValue("@today", todayString);

                var rolloutItems = new Dictionary<int, (string ItemName, decimal Price, int Available)>();

                using (var reader = rolloutCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int rolloutId = reader.GetInt32("rollout_id");
                        string itemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? "Unnamed Item" : reader.GetString("item_name").Trim();
                        decimal price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0m : reader.GetDecimal("price");
                        int available = reader.IsDBNull(reader.GetOrdinal("available")) ? 0 : reader.GetInt32("available");

                        rolloutItems[rolloutId] = (itemName, price, available);
                    }
                }

                string detailedFeedbackQuery = @"
            SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
            FROM Feedback f
            JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id
            WHERE f.item_name IN (
                SELECT item_name 
                FROM RolloutItems 
                WHERE available = 1 AND DATE(date_rolled_out) = @today
            )";

                MySqlCommand feedbackCmd = new MySqlCommand(detailedFeedbackQuery, connection);
                feedbackCmd.Parameters.AddWithValue("@today", todayString);

                var detailedFeedbackDict = new Dictionary<string, List<(double Rating, string Comments, DateTime CreatedAt)>>();

                using (var reader = feedbackCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemName = reader.GetString("item_name").Trim();
                        double rating = reader.IsDBNull(reader.GetOrdinal("rating")) ? 0.0 : reader.GetDouble("rating");
                        string comments = reader.IsDBNull(reader.GetOrdinal("comments")) ? "" : reader.GetString("comments");
                        DateTime createdAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.MinValue : reader.GetDateTime("created_at");

                        if (!detailedFeedbackDict.ContainsKey(itemName))
                        {
                            detailedFeedbackDict[itemName] = new List<(double Rating, string Comments, DateTime CreatedAt)>();
                        }

                        detailedFeedbackDict[itemName].Add((rating, comments, createdAt));
                    }
                }

                StringBuilder response = new StringBuilder();

                foreach (var item in rolloutItems)
                {
                    int rolloutId = item.Key;
                    string itemName = item.Value.ItemName;
                    decimal price = item.Value.Price;
                    int available = item.Value.Available;

                    response.AppendLine($"Rollout ID: {rolloutId}, Item Name: {itemName}, Price: {price:F2}, Available: {available}");

                    if (detailedFeedbackDict.ContainsKey(itemName))
                    {
                        var feedbackEntries = detailedFeedbackDict[itemName];
                        var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                        response.AppendLine($"Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}, Recommendation: {recommendation}");
                    }

                }

                return response.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching rollout items: {ex.Message}";
            }
            finally
            {
                Console.WriteLine("Closing connection...");
            }
        }

        public static string FetchMenu(MySqlConnection connection)
        {
            try
            {

                string menuQuery = "SELECT item_id, name, price, available FROM MenuItem";
                MySqlCommand menuCmd = new MySqlCommand(menuQuery, connection);

                var menuItems = new Dictionary<int, (string Name, decimal Price, int Available)>();

                using (var reader = menuCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int itemId = reader.GetInt32("item_id");
                        string name = reader.GetString("name");
                        decimal price = reader.GetDecimal("price");
                        int available = reader.GetInt32("available");

                        menuItems[itemId] = (name, price, available);
                    }
                }

                string feedbackQuery = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
                                       "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";
                MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

                var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

                using (var reader = feedbackCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemName = reader.GetString("item_name").Trim();
                        double rating = reader.GetDouble("rating");
                        string comment = reader.GetString("comments");
                        DateTime createdAt = reader.GetDateTime("created_at");

                        if (!feedbackDict.ContainsKey(itemName))
                        {
                            feedbackDict[itemName] = new List<(double Rating, string Comment, DateTime CreatedAt)>();
                        }

                        feedbackDict[itemName].Add((rating, comment, createdAt));
                    }
                }
                StringBuilder response = new StringBuilder();

                foreach (var item in menuItems)
                {
                    int itemId = item.Key;
                    string itemName = item.Value.Name;
                    decimal price = item.Value.Price;
                    int available = item.Value.Available;

                    response.AppendLine($"Item ID: {itemId}, Name: {itemName}, Price: {price:F2}, Available: {available}");

                    if (feedbackDict.ContainsKey(itemName))
                    {
                        var feedbackEntries = feedbackDict[itemName];

                        if (feedbackEntries.Count > 0)
                        {
                            var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.CalculateOverallSentimentsAndRatings(feedbackEntries);

                            response.AppendLine($"Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");

                        }
                    }
                    // else
                    // {
                    //     response.AppendLine($"  Average Rating: N/A, Overall Sentiment: N/A");
                    // }
                }

                return response.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching menu items: {ex.Message}";
            }
            finally
            {
                Console.WriteLine("Closing connection...");
            }
        }
        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            var overallMetrics = SentimentsAnalysis.CalculateOverallSentimentsAndRatings(entries);
            return overallMetrics;
        }


    }
}
