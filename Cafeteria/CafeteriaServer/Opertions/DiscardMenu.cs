using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Services;
using CafeteriaServer.Recommendation;

namespace CafeteriaServer.Operations
{
    public static class DiscardMenu
    {
        private static readonly MenuService menuService = new MenuService();
        private static readonly FeedbackService feedbackService = new FeedbackService();

        public static string FetchMenuItemsWithNegativeFeedback(MySqlConnection connection)
        {
            try
            {
                var menuItems = menuService.FetchMenuItems(connection);
                var feedbackDict = feedbackService.FetchFeedback(connection);

                var recommendedItems = new List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)>();

                foreach (var item in menuItems)
                {
                    int itemId = item.Key;
                    string itemName = item.Value.Name;

                    if (feedbackDict.TryGetValue(itemName, out var feedbackEntries) && feedbackEntries.Count > 0)
                    {
                        var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                        if (overallSentiment == "Negative" && averageRating <= 2.0)
                        {
                            recommendedItems.Add((itemId, item.Value.Name, item.Value.Price, item.Value.Available, averageRating, overallSentiment, recommendation));
                        }
                    }
                }

                recommendedItems.Sort((x, y) => x.AverageRating.CompareTo(y.AverageRating));
                StringBuilder response = new StringBuilder();
                response.AppendLine("Items with Negative Sentiment and Low Ratings:");

                foreach (var item in recommendedItems)
                {
                    response.AppendLine($"Item ID: {item.ItemId}, Name: {item.Name}, Price: {item.Price:F2}, Available: {item.Available}");
                    response.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
                }

                return response.ToString();
            }
            catch (Exception ex)
            {
                LogException("Error fetching menu items with negative feedback", ex);
                return $"Error fetching menu items with negative feedback: {ex.Message}";
            }
        }

        public static void RemoveMenuItem(MySqlConnection connection, string itemName)
        {
            try
            {
                string query = "DELETE FROM MenuItem WHERE name = @itemName";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@itemName", itemName);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Removed {itemName} from MenuItem.");
                    }
                    else
                    {
                        Console.WriteLine($"Item {itemName} not found in MenuItem.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogException($"Error removing menu item {itemName}", ex);
            }
        }

        public static string RollOutFoodForDetailsFeedback(MySqlConnection connection, string itemName)
        {
            try
            {
                string query = "INSERT INTO FeedbackRollout (food_item_name, rollout_date) VALUES (@foodItemName, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@foodItemName", itemName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        SendFeedbackNotification(connection, itemName);
                        return $"Food item '{itemName}' has been successfully rolled out for detailed feedback.";
                    }
                    else
                    {
                        return $"Failed to roll out '{itemName}' for detailed feedback.";
                    }
                }
            }
            catch (Exception ex)
            {
                LogException($"Error rolling out '{itemName}' for detailed feedback", ex);
                return $"Error rolling out '{itemName}' for detailed feedback: {ex.Message}";
            }
        }

        public static string SubmitDetailedFeedback(MySqlConnection connection, string itemName, string question1Response, string question2Response, string question3Response)
        {
            try
            {
                string query = "INSERT INTO DetailedFeedback (food_item_name, question_1, question_2, question_3, timestamp) " +
                               "VALUES (@foodItemName, @question1, @question2, @question3, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@foodItemName", itemName);
                    cmd.Parameters.AddWithValue("@question1", question1Response);
                    cmd.Parameters.AddWithValue("@question2", question2Response);
                    cmd.Parameters.AddWithValue("@question3", question3Response);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
                }
            }
            catch (Exception ex)
            {
                LogException($"Error submitting feedback for '{itemName}'", ex);
                return $"Error submitting feedback for '{itemName}': {ex.Message}";
            }
        }

        public static string GetDetailedFeedback(MySqlConnection connection, string itemName)
        {
            try
            {
                string query = "SELECT question_1, question_2, question_3 FROM DetailedFeedback " +
                               "WHERE food_item_name = @foodItemName ORDER BY timestamp DESC LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@foodItemName", itemName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string question1 = reader.GetString("question_1");
                            string question2 = reader.GetString("question_2");
                            string question3 = reader.GetString("question_3");

                            return $"Detailed Feedback for {itemName}:\n" +
                                   $"Q1. {question1}\n" +
                                   $"Q2. {question2}\n" +
                                   $"Q3. {question3}";
                        }
                        else
                        {
                            return $"No detailed feedback questions found for {itemName}.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException($"Error fetching detailed feedback for '{itemName}'", ex);
                return $"Error fetching detailed feedback for '{itemName}': {ex.Message}";
            }
        }

        public static string CheckFeedbackRollout(MySqlConnection connection, string itemName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM FeedbackRollout " +
                               "WHERE food_item_name = @foodItemName AND rollout_date >= CURDATE()";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@foodItemName", itemName);
                    long rolloutCount = (long)cmd.ExecuteScalar();

                    return rolloutCount > 0
                        ? $"Food item '{itemName}' is currently rolled out for detailed feedback."
                        : $"Food item '{itemName}' is not currently rolled out for detailed feedback.";
                }
            }
            catch (Exception ex)
            {
                LogException($"Error checking feedback rollout for '{itemName}'", ex);
                return $"Error checking feedback rollout for '{itemName}': {ex.Message}";
            }
        }

        public static string FetchRolloutFeedbackItems(MySqlConnection connection)
        {
            try
            {
                string query = "SELECT food_item_name FROM FeedbackRollout " +
                               "WHERE rollout_date >= CURDATE()";

                StringBuilder feedbackItems = new StringBuilder();
                feedbackItems.AppendLine("Available items for detailed feedback:");

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string foodItemName = reader.GetString("food_item_name");
                            feedbackItems.AppendLine(foodItemName);
                        }
                    }
                    else
                    {
                        feedbackItems.AppendLine("No items available for detailed feedback.");
                    }
                }

                return feedbackItems.ToString();
            }
            catch (Exception ex)
            {
                LogException("Error fetching rollout feedback items", ex);
                return $"Error fetching rollout feedback items: {ex.Message}";
            }
        }

        public static string GetDetailedFeedbackQuestions(string itemName)
        {
            try
            {
                string question1 = $"Q1. What didn’t you like about {itemName}?";
                string question2 = $"Q2. How would you like {itemName} to taste?";
                string question3 = $"Q3. Share your mom’s recipe.";

                return $"Detailed Feedback Questions for {itemName}:\n" +
                       $"{question1}\n" +
                       $"{question2}\n" +
                       $"{question3}";
            }
            catch (Exception ex)
            {
                LogException($"Error fetching detailed feedback questions for '{itemName}'", ex);
                return $"Error fetching detailed feedback questions for '{itemName}': {ex.Message}";
            }
        }

        public static string SendFeedbackNotification(MySqlConnection connection, string foodItem)
        {
            try
            {
                string message = $"We are trying to improve your experience with {foodItem}. " +
                                 "Please provide your feedback and help us.\n" +
                                 $"Q1. What didn’t you like about {foodItem}?\n" +
                                 $"Q2. How would you like {foodItem} to taste?\n" +
                                 $"Q3. Share your mom’s recipe.";

                int userTypeForEmployees = 3;
                string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) " +
                               "VALUES (@message, @userTypeId, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.Parameters.AddWithValue("@userTypeId", userTypeForEmployees);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return $"{rowsAffected} notifications sent to employees.";
                }
            }
            catch (Exception ex)
            {
                LogException($"Error sending feedback notification for '{foodItem}'", ex);
                return $"Error sending feedback notification for '{foodItem}': {ex.Message}";
            }
        }

        public static string FetchNotificationsForEmployees(int userTypeId, MySqlConnection connection)
        {
            try
            {
                string query = "SELECT message, notificationDateTime FROM Notifications " +
                               "WHERE userType_id = @userTypeId AND DATE(notificationDateTime) = CURDATE() " +
                               "ORDER BY notificationDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userTypeId", userTypeId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            StringBuilder sb = new StringBuilder();
                            while (reader.Read())
                            {
                                string message = reader.GetString("message");
                                DateTime timestamp = reader.GetDateTime("notificationDateTime");
                                sb.AppendLine($"{timestamp}: {message}");
                            }
                            return sb.ToString();
                        }
                        else
                        {
                            return "No new notifications for today.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException($"Error fetching notifications for employees", ex);
                return $"Error fetching notifications for employees: {ex.Message}";
            }
        }

        private static void LogException(string message, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {message}\nDetails: {ex.Message}");
        }
    }
}
