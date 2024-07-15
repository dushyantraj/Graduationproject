using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Recommendation;
using CafeteriaServer.Repositories;
using MySql.Data.MySqlClient;

namespace CafeteriaServer.Services
{
    public class DiscardMenuService
    {
        private readonly MenuService _menuService;
        private readonly FeedbackService _feedbackService;
        private readonly DiscardMenuRepository _discardMenuRepository;

        public DiscardMenuService(MySqlConnection connection)
        {

            _menuService = new MenuService();

            _feedbackService = new FeedbackService(connection);
            _discardMenuRepository = new DiscardMenuRepository();
        }

        public string FetchMenuItemsWithNegativeFeedback(MySqlConnection dbConnection)
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

        public string RemoveMenuItem(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                return _discardMenuRepository.RemoveMenuItem(dbConnection, itemName);
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
                var result = _discardMenuRepository.RollOutFoodForDetailedFeedback(dbConnection, itemName);

                if (result.Contains("successfully"))
                {
                    SendFeedbackNotification(dbConnection, itemName);
                }

                return result;
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
                return _discardMenuRepository.SubmitDetailedFeedback(dbConnection, itemName, question1Response, question2Response, question3Response);
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
                return _discardMenuRepository.GetLatestDetailedFeedback(dbConnection, itemName);
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
                return _discardMenuRepository.CheckFeedbackRolloutStatus(dbConnection, itemName);
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
                return _discardMenuRepository.FetchCurrentRolloutFeedbackItems(dbConnection);
            }
            catch (Exception ex)
            {
                return HandleError("Error fetching rollout feedback items", ex);
            }
        }

        public string GetFeedbackQuestionsForItem(string itemName)
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

        public string SendFeedbackNotification(MySqlConnection dbConnection, string itemName)
        {
            try
            {
                string message = $"We are trying to improve your experience with {itemName}. " +
                                 "Please provide your feedback and help us.\n" +
                                 $"Q1. What didn’t you like about {itemName}?\n" +
                                 $"Q2. How would you like {itemName} to taste?\n" +
                                 $"Q3. Share your mom’s recipe.";

                const int userTypeForEmployees = 3;
                return _discardMenuRepository.SendFeedbackNotification(dbConnection, message, userTypeForEmployees);
            }
            catch (Exception ex)
            {
                return HandleError($"Error sending feedback notification for '{itemName}'", ex);
            }
        }

        public string FetchTodayNotificationsForEmployees(MySqlConnection dbConnection, int userTypeId)
        {
            try
            {
                return _discardMenuRepository.FetchTodayNotificationsForEmployees(dbConnection, userTypeId);
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
