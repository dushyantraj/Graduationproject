using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CafeteriaServer.Models.DTO;
using CafeteriaServer.Recommendation;

namespace CafeteriaServer.Services
{
    public class RecommendationService
    {
        public static List<MenuItemDTO> GetRecommendedItems(
          Dictionary<int, ItemDTO> menuItems,
          Dictionary<string, List<FeedbackDTO>> feedbackDict)
        {
            var recommendedItems = new List<MenuItemDTO>();

            foreach (var item in menuItems)
            {
                int itemId = item.Key;
                string itemName = item.Value.Name;

                if (feedbackDict.ContainsKey(itemName))
                {
                    var feedbackEntries = feedbackDict[itemName];

                    if (feedbackEntries.Count > 0)
                    {
                        var (averageRating, overallSentiment, recommendation) = SentimentsAnalysis.AnalyzeSentimentsAndRatings(feedbackEntries);

                        if (overallSentiment == "Positive")
                        {
                            var menuItemDTO = new MenuItemDTO
                            {
                                ItemId = itemId,
                                ItemDTO = item.Value,
                                AverageRating = averageRating,
                                OverallSentiment = overallSentiment,
                                Recommendation = recommendation
                            };

                            recommendedItems.Add(menuItemDTO);
                        }
                    }
                }
            }

            recommendedItems.Sort((x, y) => y.AverageRating.CompareTo(x.AverageRating));
            return recommendedItems;
        }


        private static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<FeedbackDTO> entries)
        {
            double averageRating = entries.Average(e => e.Rating);
            string overallSentiment = averageRating > 4 ? "Positive" : "Neutral"; // Simplified sentiment analysis
            string recommendation = averageRating > 4 ? "Highly Recommended" : "Recommended";

            return (averageRating, overallSentiment, recommendation);
        }

        public string FormatRecommendedItemsResponse(
            List<MenuItemDTO> recommendedItems)
        {
            var response = new StringBuilder();
            response.AppendLine("Recommended Items with Positive Sentiment:");

            foreach (var item in recommendedItems)
            {
                response.AppendLine($"Item ID: {item.ItemId}, Name: {item.ItemDTO.Name}, Price: {item.ItemDTO.Price:F2}, Available: {item.ItemDTO.Available}");
                response.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
            }

            return response.ToString();
        }
    }
}