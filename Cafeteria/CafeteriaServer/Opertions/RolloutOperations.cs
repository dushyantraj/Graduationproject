using System;
using System.Collections.Generic;
using System.Text;
using CafeteriaServer.Models;
using CafeteriaServer.Services;
using MySql.Data.MySqlClient;
using CafeteriaServer.Repositories;
using CafeteriaServer.Models.DTO;
using CafeteriaServer.Recommendation;

namespace CafeteriaServer.Operations
{
    public class RolloutOperations
    {
        private readonly MySqlConnection _connection;
        private readonly UserProfileService _userProfileService;
        private readonly RolloutService _rolloutService;
        private readonly FeedbackService _feedbackService;

        public RolloutOperations(MySqlConnection connection)
        {
            _connection = connection;
            _userProfileService = new UserProfileService(connection);
            _rolloutService = new RolloutService(connection);
            _feedbackService = new FeedbackService();
        }

        public string FetchRolloutItemsWithFeedback(string username)
        {
            try
            {
                int userId = GetUserIdByUsername(username);
                if (userId == -1)
                {
                    return "Error: User not found.";
                }

                DateTime today = DateTime.Today;
                string todayString = today.ToString("yyyy-MM-dd");

                var userProfile = _userProfileService.FetchUserProfile(userId);
                if (userProfile == null)
                {
                    return "Error: User profile not found.";
                }

                string foodTypePreference = userProfile.Preference;
                string cuisinePreference = userProfile.CuisinePreference;
                string spiceLevel = userProfile.SpiceLevel;

                // Fetch preferred items first
                var preferredRolloutItems = _rolloutService.FetchPreferredRolloutItems(todayString, foodTypePreference, cuisinePreference, spiceLevel);

                // Fetch all items excluding those already shown as preferred
                var allRolloutItems = _rolloutService.FetchAllRolloutItems(todayString, foodTypePreference, cuisinePreference, spiceLevel);

                var detailedFeedbackDict = _feedbackService.FetchDetailedFeedback(_connection, todayString);

                var context = new RolloutRecommendationContext
                {
                    RolloutItems = preferredRolloutItems,
                    DetailedFeedbackDict = detailedFeedbackDict
                };

                var recommendedItems = RecommendItems(context);

                return GenerateResponse(recommendedItems, allRolloutItems);
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

        private List<RecommendedItemDTO> RecommendItems(RolloutRecommendationContext context)
        {
            var recommendedItems = new List<RecommendedItemDTO>();

            foreach (var item in context.RolloutItems)
            {
                int rolloutId = item.Key;
                string itemName = item.Value.ItemName;
                decimal price = item.Value.Price;
                int available = item.Value.Available;

                if (context.DetailedFeedbackDict.ContainsKey(itemName))
                {
                    var feedbackEntries = context.DetailedFeedbackDict[itemName];
                    var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                    if (overallSentiment == "Positive")
                    {
                        recommendedItems.Add(new RecommendedItemDTO
                        {
                            RolloutId = rolloutId,
                            ItemName = itemName,
                            Price = price,
                            Available = available,
                            AverageRating = averageRating,
                            OverallSentiment = overallSentiment,
                            Recommendation = recommendation
                        });
                    }
                }
            }

            recommendedItems.Sort((x, y) => y.AverageRating.CompareTo(x.AverageRating));

            return recommendedItems;
        }

        private string GenerateResponse(List<RecommendedItemDTO> recommendedItems, Dictionary<int, RolloutItem> allRolloutItems)
        {
            StringBuilder response = new StringBuilder();

            // Recommended items

            foreach (var item in recommendedItems)
            {
                response.AppendLine("Items Match with Your Preferences:");
                response.AppendLine($"Rollout ID: {item.RolloutId}, Item Name: {item.ItemName}, Price: {item.Price:F2}, Available: {item.Available}");
                response.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
            }

            // Other items
            response.AppendLine("\nTodays Items:\n");
            foreach (var item in allRolloutItems)
            {
                // Skip items already listed as recommended
                if (recommendedItems.Exists(recommended => recommended.RolloutId == item.Key))
                    continue;

                response.AppendLine($"Rollout ID: {item.Key}, Item Name: {item.Value.ItemName}, Price: {item.Value.Price:F2}, Available: {item.Value.Available}");
            }


            return response.ToString();
        }

        private int GetUserIdByUsername(string username)
        {
            var profileRepository = new EmployeeProfileRepository();
            return profileRepository.GetUserIdByUsername(_connection, username);
        }
    }

    public class RolloutRecommendationContext
    {
        public Dictionary<int, RolloutItem> RolloutItems { get; set; }
        public Dictionary<string, List<FeedbackDTO>> DetailedFeedbackDict { get; set; }
    }

}
