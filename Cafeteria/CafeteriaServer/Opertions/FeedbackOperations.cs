using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Collections.Generic;
using CafeteriaServer.Recommendation;
namespace CafeteriaServer.Operations
{
    public static class FeedbackOperations
    {
       
     public static string FillFeedbackForm(MySqlConnection connection, string itemName, int rating, string comments)
{
    try
    {
        FetchFeedbackItems(connection);

        string selectQuery = "SELECT feedback_id FROM Feedback WHERE item_name = @itemName";
        MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
        selectCmd.Parameters.AddWithValue("@itemName", itemName);

        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }

        object feedbackIdObj = selectCmd.ExecuteScalar();

        if (feedbackIdObj != null)
        {
            int feedbackId = Convert.ToInt32(feedbackIdObj);

            string insertQuery = "INSERT INTO FeedbackDetails (feedback_id, rating, comments) VALUES (@feedbackId, @rating, @comments)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@feedbackId", feedbackId);
            insertCmd.Parameters.AddWithValue("@rating", rating);
            insertCmd.Parameters.AddWithValue("@comments", comments);

            int rowsAffected = insertCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                UpdateOverallSentiment(connection, itemName);
                return "Feedback submitted successfully.";
            }
            else
            {
                return "Failed to submit feedback.";
            }
        }
        else
        {
            return "Item not found. Please provide valid food item name.";
        }
    }
    catch (Exception ex)
    {
        return $"Error filling feedback form: {ex.Message}";
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}

private static List<(double Rating, string Comment, DateTime CreatedAt)> ConvertToEntries(List<string> comments)
{
    var entries = new List<(double Rating, string Comment, DateTime CreatedAt)>();

    foreach (var comment in comments)
    {
        entries.Add((3.0, comment, DateTime.Now)); 
    }

    return entries;
}


private static void UpdateOverallSentiment(MySqlConnection connection, string itemName)
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
 public static string SubmitFeedback(MySqlConnection connection, string itemName)
        {
            try
            {
                itemName = itemName.Trim();

                if (string.IsNullOrEmpty(itemName))
                {
                    return "Item name cannot be empty.";
                }

                string query = "INSERT INTO Feedback (item_name) VALUES (@itemName)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@itemName", itemName);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
            }
            catch (Exception ex)
            {
                return "Error submitting feedback: " + ex.Message;
            }
        }
        public static string FetchFeedbackItems(MySqlConnection connection)
{
    try
    {
        StringBuilder sb = new StringBuilder();

        string todayDateString = DateTime.Today.ToString("yyyy-MM-dd");

        string query = @"
            SELECT item_name 
            FROM Feedback 
            WHERE DATE(created_at) = @todayDate";

        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@todayDate", todayDateString);

        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }

        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string itemName = reader["item_name"].ToString();
            sb.AppendLine(itemName);
        }

        reader.Close();
        return sb.ToString();
    }
    catch (Exception ex)
    {
        return $"Error fetching feedback items: {ex.Message}";
    }
    finally
    {
        if (connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}


        public static string FetchFeedbackDetails(MySqlConnection connection, string itemName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT f.item_name, fd.rating, fd.comments, fd.created_at FROM Feedback f " +
                               "JOIN FeedbackDetails fd ON f.feedback_id = fd.feedback_id " +
                               "WHERE f.item_name = @itemName";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@itemName", itemName);
                MySqlDataReader reader = cmd.ExecuteReader();

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

                reader.Close();

                if (sb.Length == 0)
                {
                    sb.AppendLine($"No feedback found for item: {itemName}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error fetching feedback details: {ex.Message}";
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
