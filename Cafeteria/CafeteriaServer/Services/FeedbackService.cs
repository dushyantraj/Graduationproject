using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Recommendation;
namespace CafeteriaServer.Services
{
    public class FeedbackService
    {
        public Dictionary<string, List<(double Rating, string Comments, DateTime CreatedAt)>> FetchDetailedFeedback(MySqlConnection connection, string todayString)
        {
            string query = @"
                SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                FROM Feedback f
                JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id
                WHERE f.item_name IN (
                    SELECT item_name 
                    FROM RolloutItems 
                    WHERE available = 1 AND DATE(date_rolled_out) = @today
                )";

            var feedbackDict = new Dictionary<string, List<(double Rating, string Comments, DateTime CreatedAt)>>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@today", todayString);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemName = reader.GetString("item_name").Trim();
                        double rating = reader.IsDBNull(reader.GetOrdinal("rating")) ? 0.0 : reader.GetDouble("rating");
                        string comments = reader.IsDBNull(reader.GetOrdinal("comments")) ? "" : reader.GetString("comments");
                        DateTime createdAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.MinValue : reader.GetDateTime("created_at");

                        if (!feedbackDict.ContainsKey(itemName))
                        {
                            feedbackDict[itemName] = new List<(double Rating, string Comments, DateTime CreatedAt)>();
                        }

                        feedbackDict[itemName].Add((rating, comments, createdAt));
                    }
                }
            }

            return feedbackDict;
        }
        public Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>> FetchAllFeedback(MySqlConnection connection)
        {
            string query = @"
                SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                FROM Feedback f
                JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";

            var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemName = reader.GetString("item_name").Trim();
                        double rating = reader.GetDouble("rating");
                        string comments = reader.GetString("comments");
                        DateTime createdAt = reader.GetDateTime("created_at");

                        if (!feedbackDict.ContainsKey(itemName))
                        {
                            feedbackDict[itemName] = new List<(double Rating, string Comment, DateTime CreatedAt)>();
                        }

                        feedbackDict[itemName].Add((rating, comments, createdAt));
                    }
                }
            }

            return feedbackDict;
        }
        public Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>> FetchFeedback(MySqlConnection connection)
        {
            const string feedbackQuery = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
                                         "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";
            var feedbackDict = new Dictionary<string, List<(double Rating, string Comment, DateTime CreatedAt)>>();

            using (MySqlCommand cmd = new MySqlCommand(feedbackQuery, connection))
            using (var reader = cmd.ExecuteReader())
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

            return feedbackDict;
        }
        public string FillFeedbackForm(MySqlConnection connection, string itemName, int rating, string comments)
        {
            try
            {
                FetchFeedbackItems(connection); // Fetching items possibly for some purpose

                int feedbackId = GetFeedbackId(connection, itemName);

                if (feedbackId == -1)
                {
                    return "Item not found. Please provide a valid food item name.";
                }

                InsertFeedbackDetails(connection, feedbackId, rating, comments);

                UpdateOverallSentiment(connection, itemName);

                return "Feedback submitted successfully.";
            }
            catch (Exception ex)
            {
                LogException("Error filling feedback form", ex);
                return $"Error filling feedback form: {ex.Message}";
            }
        }

        public string SubmitFeedback(MySqlConnection connection, string itemName)
        {
            try
            {
                itemName = itemName.Trim();

                if (string.IsNullOrEmpty(itemName))
                {
                    return "Item name cannot be empty.";
                }

                int rowsAffected = InsertFeedback(connection, itemName);

                return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
            }
            catch (Exception ex)
            {
                LogException("Error submitting feedback", ex);
                return $"Error submitting feedback: {ex.Message}";
            }
        }

        public string FetchFeedbackItems(MySqlConnection connection)
        {
            try
            {
                string todayDateString = DateTime.Today.ToString("yyyy-MM-dd");

                string query = @"
                    SELECT item_name 
                    FROM Feedback 
                    WHERE DATE(created_at) = @todayDate";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@todayDate", todayDateString);

                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        StringBuilder sb = new StringBuilder();

                        while (reader.Read())
                        {
                            string itemName = reader["item_name"].ToString();
                            sb.AppendLine(itemName);
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Error fetching feedback items", ex);
                return $"Error fetching feedback items: {ex.Message}";
            }
        }

        public string FetchFeedbackDetails(MySqlConnection connection, string itemName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string query = @"
                    SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                    FROM Feedback f 
                    JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id 
                    WHERE f.item_name = @itemName";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemName", itemName);

                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double rating = reader.GetDouble("rating");
                            string comments = reader.GetString("comments");
                            DateTime createdAt = reader.GetDateTime("created_at");

                            sb.AppendLine($"Item: {itemName}");
                            sb.AppendLine($"  Rating: {rating}");
                            sb.AppendLine($"  Comments: {comments}");
                            sb.AppendLine($"  Date: {createdAt}");
                        }

                        if (sb.Length == 0)
                        {
                            sb.AppendLine($"No feedback found for item: {itemName}");
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Error fetching feedback details", ex);
                return $"Error fetching feedback details: {ex.Message}";
            }
        }

        private int GetFeedbackId(MySqlConnection connection, string itemName)
        {
            string selectQuery = "SELECT feedback_id FROM Feedback WHERE item_name = @itemName";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
            {
                cmd.Parameters.AddWithValue("@itemName", itemName);

                object feedbackIdObj = cmd.ExecuteScalar();

                return feedbackIdObj != null ? Convert.ToInt32(feedbackIdObj) : -1;
            }
        }

        private void InsertFeedbackDetails(MySqlConnection connection, int feedbackId, int rating, string comments)
        {
            string insertQuery = "INSERT INTO FeedbackDetails (feedback_id, rating, comments) VALUES (@feedbackId, @rating, @comments)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@feedbackId", feedbackId);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@comments", comments);

                cmd.ExecuteNonQuery();
            }
        }

        private int InsertFeedback(MySqlConnection connection, string itemName)
        {
            string insertQuery = "INSERT INTO Feedback (item_name) VALUES (@itemName)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
            {
                cmd.Parameters.AddWithValue("@itemName", itemName);

                return cmd.ExecuteNonQuery();
            }
        }

        private void UpdateOverallSentiment(MySqlConnection connection, string itemName)
        {
            string selectCommentsQuery = "SELECT comments FROM FeedbackDetails WHERE feedback_id IN " +
                                         "(SELECT feedback_id FROM Feedback WHERE item_name = @itemName)";
            MySqlCommand selectCommentsCmd = new MySqlCommand(selectCommentsQuery, connection);
            selectCommentsCmd.Parameters.AddWithValue("@itemName", itemName);

            List<string> comments = new List<string>();

            using (MySqlDataReader reader = selectCommentsCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comments.Add(reader.GetString("comments"));
                }
            }

            var entries = ConvertToEntries(comments);
            var overallMetrics = SentimentsAnalysis.AnalyzeSentimentsAndRatings(entries);

            string updateQuery = "UPDATE Feedback SET overall_sentiment = @overallSentiment " +
                                 "WHERE item_name = @itemName";
            MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@overallSentiment", overallMetrics.OverallSentiment);
            updateCmd.Parameters.AddWithValue("@itemName", itemName);

            updateCmd.ExecuteNonQuery();
        }

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
            // Extend this method to log exceptions to a file or another logging service
        }

        private List<(double Rating, string Comment, DateTime CreatedAt)> ConvertToEntries(List<string> comments)
        {
            var entries = new List<(double Rating, string Comment, DateTime CreatedAt)>();

            foreach (var comment in comments)
            {
                entries.Add((3.0, comment, DateTime.Now)); // Dummy rating for now
            }

            return entries;
        }
    }
}
