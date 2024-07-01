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
        public static List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)> GetRecommendedItems(
          Dictionary<int, (string Name, decimal Price, int Available)> menuItems,
          Dictionary<string, List<FeedbackDTO>> feedbackDict)
        {
            var recommendedItems = new List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)>();

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
                            recommendedItems.Add((itemId, itemName, item.Value.Price, item.Value.Available, averageRating, overallSentiment, recommendation));
                        }
                    }
                }
            }

            recommendedItems.Sort((x, y) => y.AverageRating.CompareTo(x.AverageRating));
            return recommendedItems;
        }


        private (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            double averageRating = entries.Average(e => e.Rating);
            string overallSentiment = averageRating > 4 ? "Positive" : "Neutral"; // Simplified sentiment analysis
            string recommendation = averageRating > 4 ? "Highly Recommended" : "Recommended";

            return (averageRating, overallSentiment, recommendation);
        }

        public string FormatRecommendedItemsResponse(
            List<(int ItemId, string Name, decimal Price, int Available, double AverageRating, string OverallSentiment, string Recommendation)> recommendedItems)
        {
            var response = new StringBuilder();
            response.AppendLine("Recommended Items with Positive Sentiment:");

            foreach (var item in recommendedItems)
            {
                response.AppendLine($"Item ID: {item.ItemId}, Name: {item.Name}, Price: {item.Price:F2}, Available: {item.Available}");
                response.AppendLine($"  Rating: {item.AverageRating:F1}, Overall Sentiment: {item.OverallSentiment}, Recommendation: {item.Recommendation}");
            }

            return response.ToString();
        }
    }
}