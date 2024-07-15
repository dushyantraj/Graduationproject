using System;
using System.Text;
using MySql.Data.MySqlClient;

namespace CafeteriaServer.Repositories
{
    public class DiscardMenuRepository
    {
        public string RemoveMenuItem(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "DELETE FROM MenuItem WHERE name = @itemName";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@itemName", itemName);
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0
                        ? $"Removed {itemName} from MenuItem."
                        : $"Item {itemName} not found in MenuItem.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error removing menu item {itemName}", ex);
            }
        }

        public string RollOutFoodForDetailedFeedback(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "INSERT INTO FeedbackRollout (food_item_name, rollout_date) VALUES (@foodItemName, NOW())";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0
                        ? $"Food item '{itemName}' has been successfully rolled out for detailed feedback."
                        : $"Failed to roll out '{itemName}' for detailed feedback.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error rolling out '{itemName}' for detailed feedback", ex);
            }
        }

        public string SubmitDetailedFeedback(MySqlConnection dbConnection, string itemName, string question1Response, string question2Response, string question3Response)
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

                    return rowsAffected > 0
                        ? $"Feedback for '{itemName}' submitted successfully."
                        : $"Failed to submit feedback for '{itemName}'.";
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error submitting feedback for '{itemName}'", ex);
            }
        }

        public string GetLatestDetailedFeedback(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "SELECT * FROM DetailedFeedback WHERE food_item_name = @foodItemName ORDER BY timestamp DESC LIMIT 1";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string question1 = reader["question_1"].ToString();
                            string question2 = reader["question_2"].ToString();
                            string question3 = reader["question_3"].ToString();
                            string timestamp = reader["timestamp"].ToString();

                            var feedback = new StringBuilder();
                            feedback.AppendLine($"Feedback for {itemName}:");
                            feedback.AppendLine($"Q1: {question1}");
                            feedback.AppendLine($"Q2: {question2}");
                            feedback.AppendLine($"Q3: {question3}");
                            feedback.AppendLine($"Timestamp: {timestamp}");

                            return feedback.ToString();
                        }
                        else
                        {
                            return $"No feedback found for '{itemName}'.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error fetching detailed feedback for '{itemName}'", ex);
            }
        }

        public string CheckFeedbackRolloutStatus(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string query = "SELECT * FROM FeedbackRollout WHERE food_item_name = @foodItemName ORDER BY rollout_date DESC LIMIT 1";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@foodItemName", itemName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string rolloutDate = reader["rollout_date"].ToString();
                            return $"'{itemName}' was rolled out for detailed feedback on {rolloutDate}.";
                        }
                        else
                        {
                            return $"No rollout record found for '{itemName}'.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError($"Error checking feedback rollout for '{itemName}'", ex);
            }
        }

        public string FetchCurrentRolloutFeedbackItems(MySqlConnection dbConnection)
        {
            try
            {
                //  string query = "SELECT food_item_name, rollout_date FROM FeedbackRollout ORDER BY rollout_date DESC";
                string query = "SELECT food_item_name FROM FeedbackRollout " +
                                            "WHERE rollout_date >= CURDATE()";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var items = new StringBuilder();
                        items.AppendLine("Available items for detailed feedback:");

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string foodItemName = reader.GetString("food_item_name");
                                items.AppendLine(foodItemName);
                            }
                        }
                        else
                        {
                            items.AppendLine("No items available for detailed feedback.");
                        }
                        return items.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError("Error fetching rollout feedback items", ex);
            }

        }

        public string SendFeedbackNotification(MySqlConnection dbConnection, string message, int userTypeForEmployees)
        {
            try
            {
                string query = "INSERT INTO Notifications (user_type_id, message, timestamp) VALUES (@userType, @message, NOW())";

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@userType", userTypeForEmployees);
                    command.Parameters.AddWithValue("@message", message);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0
                        ? "Feedback notification sent successfully."
                        : "Failed to send feedback notification.";
                }
            }
            catch (Exception ex)
            {
                return HandleError("Error sending feedback notification", ex);
            }
        }

        public string FetchTodayNotificationsForEmployees(MySqlConnection dbConnection, int userTypeId)
        {
            try
            {
                string query = "SELECT message, notificationDateTime FROM Notifications " +
                              "WHERE userType_id = @userTypeId AND DATE(notificationDateTime) = CURDATE() " +
                              "ORDER BY notificationDateTime DESC";
                Console.WriteLine($"Executing query: {query}");
                Console.WriteLine($"With parameters: userTypeId = {userTypeId}, date = {DateTime.Now.ToString("yyyy-MM-dd")}");

                using (var command = new MySqlCommand(query, dbConnection))
                {
                    command.Parameters.AddWithValue("@userTypeId", userTypeId);

                    using (var reader = command.ExecuteReader())
                    {
                        var notifications = new StringBuilder();
                        notifications.AppendLine("Today's Notifications:");

                        while (reader.Read())
                        {
                            string message = reader["message"].ToString();
                            string timestamp = reader["notificationDateTime"].ToString();
                            notifications.AppendLine($"Message: {message}, Timestamp: {timestamp}");
                        }

                        return notifications.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError("Error fetching notifications for employees", ex);
            }
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
