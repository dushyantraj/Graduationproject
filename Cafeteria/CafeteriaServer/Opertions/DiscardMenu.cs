
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
        private static readonly MenuService _menuService = new MenuService();
        private static readonly FeedbackService _feedbackService = new FeedbackService();

        public static string FetchMenuItemsWithNegativeFeedback(MySqlConnection dbConnection)
        {
            try
            {
                var menuItems = _menuService.FetchMenuItems(dbConnection);
                var feedbackEntries = _feedbackService.FetchFeedback(dbConnection);

                var itemsToRemove = new List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)>();

                foreach (var menuItem in menuItems)
                {
                    int itemId = menuItem.Key;
                    string itemName = menuItem.Value.Name;

                    if (feedbackEntries.TryGetValue(itemName, out var itemFeedback) && itemFeedback.Count > 0)
                    {
                        var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(itemFeedback);

                        if (overallSentiment == "Negative" && averageRating <= 2.0)
                        {
                            itemsToRemove.Add((itemId, itemName, menuItem.Value.Price, menuItem.Value.Available, averageRating, overallSentiment, recommendation));
                        }
                    }
                }

                itemsToRemove.Sort((x, y) => x.AverageRating.CompareTo(y.AverageRating));
                return BuildNegativeFeedbackReport(itemsToRemove);
            }
            catch (Exception ex)
            {
                return HandleError("Error fetching menu items with negative feedback", ex);
            }
        }

        public static void RemoveMenuItem(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "DELETE FROM MenuItem WHERE name = @itemName";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);
                    int rowsAffected = command.ExecuteNonQuery();

                    Console.WriteLine(rowsAffected > 0
                        ? $"Removed {itemName} from MenuItem."
                        : $"Item {itemName} not found in MenuItem.");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error removing menu item {itemName}", ex);
            }
        }

        public static string RollOutFoodForDetailedFeedback(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "INSERT INTO FeedbackRollout (food_item_name, rollout_date) VALUES (@foodItemName, NOW())";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        SendFeedbackNotification(dbConnection, itemName);
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
                return HandleError($"Error rolling out '{itemName}' for detailed feedback", ex);
            }
        }

        public static string SubmitDetailedFeedback(MySqlConnection dbConnection, string itemName, string question1Response, string question2Response, string question3Response)
        {
            try
            {
                string query = "INSERT INTO DetailedFeedback (food_item_name, question_1, question_2, question_3, timestamp) " +
                               "VALUES (@foodItemName, @question1, @question2, @question3, NOW())";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);
                    command.Parameters.AddWithValue("@question1", question1Response);
                    command.Parameters.AddWithValue("@question2", question2Response);
                    command.Parameters.AddWithValue("@question3", question3Response);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Feedback submitted successfully." : "Failed to submit feedback.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error submitting feedback for '{itemName}'", ex);
            }
        }

        public static string GetLatestDetailedFeedback(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "SELECT question_1, question_2, question_3 FROM DetailedFeedback " +
                               "WHERE food_item_name = @foodItemName ORDER BY timestamp DESC LIMIT 1";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);

                    using (var reader = command.ExecuteReader())
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
                            return $"No detailed feedback found for {itemName}.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error fetching detailed feedback for '{itemName}'", ex);
            }
        }

        public static string CheckFeedbackRolloutStatus(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM FeedbackRollout " +
                               "WHERE food_item_name = @foodItemName AND rollout_date >= CURDATE()";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);
                    long rolloutCount = (long)command.ExecuteScalar();

                    return rolloutCount > 0
                        ? $"Food item '{itemName}' is currently rolled out for detailed feedback."
                        : $"Food item '{itemName}' is not currently rolled out for detailed feedback.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error checking feedback rollout for '{itemName}'", ex);
            }
        }

        public static string FetchCurrentRolloutFeedbackItems(MySqlConnection dbConnection)
        {
            try
            {
                string query = "SELECT food_item_name FROM FeedbackRollout " +
                               "WHERE rollout_date >= CURDATE()";

                using (var command = new MySqlCommand(query, dbConnection))
                using (var reader = command.ExecuteReader())
                {
                    StringBuilder feedbackItems = new StringBuilder();
                    feedbackItems.AppendLine("Available items for detailed feedback:");

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

                    return feedbackItems.ToString();
                }
            }
            catch (Exception ex)
            {
                return HandleError("Error fetching rollout feedback items", ex);
            }
        }

        public static string GetFeedbackQuestionsForItem(string itemName)
        {
            try
            {
                return $"Detailed Feedback Questions for {itemName}:\n" +
                       $"Q1. What didn’t you like about {itemName}?\n" +
                       $"Q2. How would you like {itemName} to taste?\n" +
                       $"Q3. Share your mom’s recipe.";
            }
            catch (Exception ex)
            {
                return HandleError($"Error fetching detailed feedback questions for '{itemName}'", ex);
            }
        }

        public static string SendFeedbackNotification(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string message = $"We are trying to improve your experience with {itemName}. " +
                                 "Please provide your feedback and help us.\n" +
                                 $"Q1. What didn’t you like about {itemName}?\n" +
                                 $"Q2. How would you like {itemName} to taste?\n" +
                                 $"Q3. Share your mom’s recipe.";

                const int userTypeForEmployees = 3;
                string query = "INSERT INTO Notifications (message, userType_id, notificationDateTime) " +
                               "VALUES (@message, @userTypeId, NOW())";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@message", message);
                    command.Parameters.AddWithValue("@userTypeId", userTypeForEmployees);
                    int rowsAffected = command.ExecuteNonQuery();
                    return $"{rowsAffected} notifications sent to employees.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error sending feedback notification for '{itemName}'", ex);
            }
        }

        public static string FetchTodayNotificationsForEmployees(MySqlConnection dbConnection, int userTypeId)
        {
            try
            {
                string query = "SELECT message, notificationDateTime FROM Notifications " +
                               "WHERE userType_id = @userTypeId AND DATE(notificationDateTime) = CURDATE() " +
                               "ORDER BY notificationDateTime DESC";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@userTypeId", userTypeId);

                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder notifications = new StringBuilder();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string message = reader.GetString("message");
                                DateTime timestamp = reader.GetDateTime("notificationDateTime");
                                notifications.AppendLine($"{timestamp}: {message}");
                            }
                        }
                        else
                        {
                            notifications.AppendLine("No new notifications for today.");
                        }
                        return notifications.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error fetching notifications for employees", ex);
            }
        }

        private static string BuildNegativeFeedbackReport(List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)> items)
        {
            var report = new StringBuilder();
            report.AppendLine("Items with Negative Sentiment and Low Ratings:");
            foreach (var item in items)
            {
                report.AppendLine($"Item ID: {item.ItemId}, Name: {item.Name}, Price: {item.Price:F2}, Available: {item.Available}");
                report.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
            }
            return report.ToString();
        }

        private static string HandleError(string errorMessage, Exception ex)
        {
            LogError(errorMessage, ex);
            return $"{errorMessage}: {ex.Message}";
        }

        private static void LogError(string errorMessage, Exception ex)
        {
            Console.WriteLine($"Exception occurred: {errorMessage}\nDetails: {ex.Message}");
        }
    }
}
