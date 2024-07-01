using MySql.Data.MySqlClient;
using CafeteriaServer.Models;
using CafeteriaServer.Services;
using CafeteriaServer.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Recommendation;
using CafeteriaServer.Repositories;
namespace CafeteriaServer.Operations
{
    public class RolloutOperations
    {
        private readonly UserProfileService _userProfileService;
        private readonly RolloutService _rolloutService;
        private readonly FeedbackService _feedbackService;

        public RolloutOperations()
        {
            _userProfileService = new UserProfileService();
            _rolloutService = new RolloutService();
            _feedbackService = new FeedbackService();
        }

        public string FetchRolloutItemsWithFeedback(MySqlConnection connection, string username)
        {
            try
            {
                var profileRepository = new EmployeeProfileRepository();
                int userId = profileRepository.GetUserIdByUsername(connection, username);
                if (userId == -1)
                {
                    return "Error: User not found.";
                }

                DateTime today = DateTime.Today;
                string todayString = today.ToString("yyyy-MM-dd");

                var userProfile = _userProfileService.FetchUserProfile(connection, userId);
                if (userProfile == null)
                {
                    return "Error: User profile not found.";
                }

                string foodTypePreference = userProfile.Preference;

                var rolloutItems = _rolloutService.FetchRolloutItems(connection, todayString, foodTypePreference);
                var detailedFeedbackDict = _feedbackService.FetchDetailedFeedback(connection, todayString);

                var recommendedItems = RecommendItems(rolloutItems, detailedFeedbackDict);

                return GenerateResponse(recommendedItems);
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

        private List<(int RolloutId, string ItemName, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)> RecommendItems(
            Dictionary<int, RolloutItem> rolloutItems,
            Dictionary<string, List<(double Rating, string Comments, DateTime CreatedAt)>> detailedFeedbackDict)
        {
            var recommendedItems = new List<(int RolloutId, string ItemName, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)>();

            foreach (var item in rolloutItems)
            {
                int rolloutId = item.Key;
                string itemName = item.Value.ItemName;
                decimal price = item.Value.Price;
                int available = item.Value.Available;

                if (detailedFeedbackDict.ContainsKey(itemName))
                {
                    var feedbackEntries = detailedFeedbackDict[itemName];
                    var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                    if (overallSentiment == "Positive")
                    {
                        recommendedItems.Add((rolloutId, itemName, price, available, averageRating, overallSentiment, recommendation));
                    }
                }
            }

            recommendedItems.Sort((x, y) => y.AverageRating.CompareTo(x.AverageRating));

            return recommendedItems;
        }
        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            var overallMetrics = SentimentsAnalysis.AnalyzeSentimentsAndRatings(entries);
            return overallMetrics;
        }
        private string GenerateResponse(List<(int RolloutId, string ItemName, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)> recommendedItems)
        {
            StringBuilder response = new StringBuilder();
            response.AppendLine("Recommended Items with Positive Sentiment:");

            foreach (var item in recommendedItems)
            {
                response.AppendLine($"Rollout ID: {item.RolloutId}, Item Name: {item.ItemName}, Price: {item.Price:F2}, Available: {item.Available}");
                response.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
            }

            return response.ToString();
        }
    }
}
