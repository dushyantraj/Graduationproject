using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data;
using CafeteriaServer.Recommendation;

namespace CafeteriaServer.Operations
{
    public static class RolloutOperations
    {
        public static string FetchRolloutItemsWithFeedback(MySqlConnection connection)
        {
            try
            {

                string rolloutQuery = "SELECT rollout_id, item_name, price, available FROM RolloutItems WHERE available = 1";
                MySqlCommand rolloutCmd = new MySqlCommand(rolloutQuery, connection);

                var rolloutItems = new Dictionary<int, (string ItemName, decimal Price, int Available)>();

                using (var reader = rolloutCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int rolloutId = reader.GetInt32("rollout_id");
                        string itemName = reader.IsDBNull(reader.GetOrdinal("item_name")) ? "Unnamed Item" : reader.GetString("item_name");
                        decimal price = reader.IsDBNull(reader.GetOrdinal("price")) ? 0.0m : reader.GetDecimal("price");
                        int available = reader.IsDBNull(reader.GetOrdinal("available")) ? 0 : reader.GetInt32("available");

                        rolloutItems[rolloutId] = (itemName, price, available);
                    }
                }

                string feedbackQuery = @"
            SELECT f.item_name, AVG(fd.rating) AS averageRating, COUNT(fd.feedback_id) AS totalFeedback,
                   SUM(CASE WHEN fd.rating >= 4 THEN 1 ELSE 0 END) AS positiveCount,
                   SUM(CASE WHEN fd.rating < 2 THEN 1 ELSE 0 END) AS negativeCount
            FROM Feedback f
            JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id
            WHERE f.item_name IN (SELECT item_name FROM RolloutItems WHERE available = 1)
            GROUP BY f.item_name";

                MySqlCommand feedbackCmd = new MySqlCommand(feedbackQuery, connection);

                var feedbackDict = new Dictionary<string, (double AverageRating, string OverallSentiment)>();

                using (var reader = feedbackCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemName = reader.GetString("item_name").Trim();
                        double averageRating = reader.IsDBNull(reader.GetOrdinal("averageRating")) ? 0.0 : reader.GetDouble("averageRating");
                        int positiveCount = reader.IsDBNull(reader.GetOrdinal("positiveCount")) ? 0 : reader.GetInt32("positiveCount");
                        int negativeCount = reader.IsDBNull(reader.GetOrdinal("negativeCount")) ? 0 : reader.GetInt32("negativeCount");
                        int totalFeedback = reader.IsDBNull(reader.GetOrdinal("totalFeedback")) ? 0 : reader.GetInt32("totalFeedback");

                        string overallSentiment = CalculateOverallSentiment(positiveCount, negativeCount, totalFeedback);

                        feedbackDict[itemName] = (averageRating, overallSentiment);
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

                    if (feedbackDict.ContainsKey(itemName))
                    {
                        var (averageRating, overallSentiment) = feedbackDict[itemName];
                        response.AppendLine($" Rating: {averageRating:F1}, Overall Sentiment: {overallSentiment}");
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
                return $"Error fetching rollout items: {ex.Message}";
            }
            finally
            {
                Console.WriteLine("Closing connection...");
            }
        }
        public static string RolloutFoodItemsForNextDay(MySqlConnection connection, string[] itemIds)
        {
            try
            {
                int successfulCount = 0;
                DateTime today = DateTime.Today;

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach (var itemIdStr in itemIds)
                    {
                        if (int.TryParse(itemIdStr, out int itemId))
                        {

                            string checkQuery = "SELECT COUNT(*) FROM RolloutItems WHERE item_id = @itemId AND date_rolled_out = @today";
                            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection, transaction);
                            checkCmd.Parameters.AddWithValue("@itemId", itemId);
                            checkCmd.Parameters.AddWithValue("@today", today);

                            long count = (long)checkCmd.ExecuteScalar();

                            if (count == 0)
                            {
                                string getItemQuery = "SELECT * FROM MenuItem WHERE item_id = @itemId";
                                MySqlCommand getItemCmd = new MySqlCommand(getItemQuery, connection, transaction);
                                getItemCmd.Parameters.AddWithValue("@itemId", itemId);

                                using (MySqlDataReader reader = getItemCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        string itemName = reader.GetString("name");
                                        decimal price = reader.GetDecimal("price");
                                        int available = reader.GetInt32("available");

                                        reader.Close();

                                        string insertQuery = "INSERT INTO RolloutItems (item_id, item_name, price, available, selected_for_next_day, date_rolled_out) " +
                                                             "VALUES (@itemId, @itemName, @price, @available, true, @today)";
                                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction);
                                        insertCmd.Parameters.AddWithValue("@itemId", itemId);
                                        insertCmd.Parameters.AddWithValue("@itemName", itemName);
                                        insertCmd.Parameters.AddWithValue("@price", price);
                                        insertCmd.Parameters.AddWithValue("@available", available);
                                        insertCmd.Parameters.AddWithValue("@today", today);

                                        int rowsAffected = insertCmd.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                            successfulCount++;
                                    }
                                    else
                                    {
                                        reader.Close();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Item with ID {itemId} has already been rolled out today.");
                            }
                        }
                    }
                    transaction.Commit();
                }

                if (successfulCount == itemIds.Length)
                    return "Items rolled out for next day successfully.";
                else if (successfulCount > 0)
                    return "Some items were already rolled out today. Only new items were added.";
                else
                    return "All selected items were already rolled out today.";
            }
            catch (Exception ex)
            {
                return "Error rolling out item: " + ex.Message;
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
                            var (averageRating, overallSentiment, recommendation) = AnalyzeSentimentsAndRatings(feedbackEntries);

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
            var overallMetrics = CalculateOverallSentimentsAndRatings(entries);
            return overallMetrics;
        }
        public static (double AverageRating, string OverallSentiment, string Recommendation) CalculateOverallSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            int positiveCount = 0;
            int negativeCount = 0;
            int neutralCount = 0;

            double totalRating = 0;

            foreach (var entry in entries)
            {
                string comment = entry.Comment.ToLower();
                int sentimentScore = 0;
                bool isNegated = false;


                string[] words = comment.Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    if (SentimentWords.NegationWords.Contains(word))
                    {
                        isNegated = true;
                        continue;
                    }

                    if (SentimentWords.PositiveWords.Contains(word))
                    {
                        sentimentScore += isNegated ? -1 : 1;
                        isNegated = false;
                    }
                    else if (SentimentWords.NegativeWords.Contains(word))
                    {
                        sentimentScore += isNegated ? 1 : -1;
                        isNegated = false;
                    }
                }

                if (sentimentScore > 0)
                {
                    positiveCount++;
                }
                else if (sentimentScore < 0)
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

            string overallSentiment = DetermineOverallSentiment(positiveCount, negativeCount, neutralCount);

            string recommendation = averageRating >= 4.0 ? "Highly recommended" :
                                    averageRating >= 3.0 ? "Recommended" :
                                    averageRating >= 2.0 ? "Average" :
                                    "Not recommended";

            return (averageRating, overallSentiment, recommendation);
        }

        private static string DetermineOverallSentiment(int positiveCount, int negativeCount, int neutralCount)
        {
            if (positiveCount > negativeCount)
            {
                return "Positive";
            }
            else if (negativeCount > positiveCount)
            {
                return "Negative";
            }
            else
            {
                return "Neutral";
            }
        }
        public static string CalculateOverallSentiment(int positiveCount, int negativeCount, int totalFeedback)
        {
            if (totalFeedback == 0) return "N/A";

            if (positiveCount > negativeCount)
            {
                return "Positive";
            }
            else if (negativeCount > positiveCount)
            {
                return "Negative";
            }
            else
            {
                return "Neutral";
            }
        }
    }
}
