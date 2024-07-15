using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CafeteriaServer.Models.DTO;

namespace CafeteriaServer.Repositories
{
    public class FeedbackRepository
    {
        private readonly MySqlConnection _connection;

        public FeedbackRepository(MySqlConnection connection)
        {
            _connection = connection;
        }
        public Dictionary<string, List<FeedbackDTO>> FetchAllFeedback(MySqlConnection connection)
        {
            const string query = @"
                SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                FROM Feedback f
                JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";

            var feedbackDict = new Dictionary<string, List<FeedbackDTO>>();

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
                            feedbackDict[itemName] = new List<FeedbackDTO>();
                        }

                        feedbackDict[itemName].Add(new FeedbackDTO
                        {
                            Rating = rating,
                            Comments = comments,
                            CreatedAt = createdAt
                        });
                    }
                }
            }

            return feedbackDict;
        }
        public Dictionary<string, List<FeedbackDTO>> FetchDetailedFeedback(string todayString)
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

            var feedbackDict = new Dictionary<string, List<FeedbackDTO>>();

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
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
                            feedbackDict[itemName] = new List<FeedbackDTO>();
                        }

                        feedbackDict[itemName].Add(new FeedbackDTO
                        {
                            Rating = rating,
                            Comments = comments,
                            CreatedAt = createdAt
                        });
                    }
                }
            }

            return feedbackDict;
        }

        public Dictionary<string, List<FeedbackDTO>> FetchAllFeedback()
        {
            string query = @"
                SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                FROM Feedback f
                JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id";

            var feedbackDict = new Dictionary<string, List<FeedbackDTO>>();

            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
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
                            feedbackDict[itemName] = new List<FeedbackDTO>();
                        }

                        feedbackDict[itemName].Add(new FeedbackDTO
                        {
                            Rating = rating,
                            Comments = comments,
                            CreatedAt = createdAt
                        });
                    }
                }
            }

            return feedbackDict;
        }

        public string FetchFeedbackItems()
        {
            try
            {
                string todayDateString = DateTime.Today.ToString("yyyy-MM-dd");

                string query = @"
            SELECT DISTINCT item_name 
            FROM Feedback 
            WHERE DATE(created_at) = @todayDate";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@todayDate", todayDateString);

                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var sb = new System.Text.StringBuilder();

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

        public string FetchFeedbackDetails(string itemName)
        {
            try
            {
                var sb = new System.Text.StringBuilder();

                string query = @"
                    SELECT f.item_name, fd.rating, fd.comments, fd.created_at 
                    FROM Feedback f 
                    JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id 
                    WHERE f.item_name = @itemName";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@itemName", itemName);

                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
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

        private void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
        public int GetFeedbackId(string itemName)
        {
            string query = "SELECT feedback_id FROM Feedback WHERE item_name = @itemName";
            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@itemName", itemName);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                return -1; // or throw an exception if feedback id not found
            }
        }

        public void InsertFeedbackDetails(int feedbackId, int rating, string comments)
        {
            string query = "INSERT INTO FeedbackDetails (feedback_id, rating, comments, created_at) VALUES (@feedbackId, @rating, @comments, NOW())";
            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@feedbackId", feedbackId);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@comments", comments);
                cmd.ExecuteNonQuery();
            }
        }

        public int InsertFeedback(string itemName)
        {
            string query = "INSERT INTO Feedback (item_name) VALUES (@itemName)";
            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@itemName", itemName);
                return cmd.ExecuteNonQuery();
            }
        }
        public void UpdateOverallSentiment(string itemName)
        {
            string selectCommentsQuery = "SELECT comments FROM FeedbackDetails WHERE feedback_id IN " +
                                         "(SELECT feedback_id FROM Feedback WHERE item_name = @itemName)";
            MySqlCommand selectCommentsCmd = new MySqlCommand(selectCommentsQuery, _connection);
            selectCommentsCmd.Parameters.AddWithValue("@itemName", itemName);

            List<string> comments = new List<string>();

            using (MySqlDataReader reader = selectCommentsCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comments.Add(reader.GetString("comments"));
                }
            }
        }

    }
}
