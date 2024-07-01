
using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeteriaServer.Recommendation
{
    public static class SentimentsAnalysis
    {
        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            double averageRating = CalculateAverageRating(entries);
            string overallSentiment = DetermineOverallSentiment(entries);
            string recommendation = GenerateRecommendation(averageRating);

            return (averageRating, overallSentiment, recommendation);
        }

        public static string CalculateOverallSentiment(int positiveCount, int negativeCount, int totalFeedback)
        {
            if (totalFeedback == 0)
                return "N/A";

            return DetermineSentimentLabel(positiveCount, negativeCount, totalFeedback - positiveCount - negativeCount);
        }

        private static double CalculateAverageRating(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            if (!entries.Any())
                return 0.0;

            return entries.Average(entry => entry.Rating);
        }

        private static string DetermineOverallSentiment(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            int positiveCount = 0;
            int negativeCount = 0;
            int neutralCount = 0;

            foreach (var entry in entries)
            {
                int sentimentScore = CalculateSentimentScore(entry.Comment);
                if (sentimentScore > 0)
                    positiveCount++;
                else if (sentimentScore < 0)
                    negativeCount++;
                else
                    neutralCount++;
            }

            return DetermineSentimentLabel(positiveCount, negativeCount, neutralCount);
        }

        private static int CalculateSentimentScore(string comment)
        {
            int sentimentScore = 0;
            bool isNegated = false;

            string[] words = comment.ToLower()
                                    .Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (SentimentWords.NegationWords.Contains(word))
                {
                    isNegated = !isNegated;  // Toggle negation for each negation word found
                    continue;
                }

                if (SentimentWords.PositiveWords.Contains(word))
                {
                    sentimentScore += isNegated ? -1 : 1;
                    isNegated = false;
                }
                else if (SentimentWords.NegativeWords.Contains(word))
                {
                    sentimentScore += isNegated ? 1 : -1;
                    isNegated = false;
                }
            }

            return sentimentScore;
        }

        private static string DetermineSentimentLabel(int positiveCount, int negativeCount, int neutralCount)
        {
            if (positiveCount > negativeCount)
                return "Positive";
            else if (negativeCount > positiveCount)
                return "Negative";
            else
                return "Neutral";
        }

        private static string GenerateRecommendation(double averageRating)
        {
            if (averageRating >= 4.0)
                return "Highly recommended";
            else if (averageRating >= 3.0)
                return "Recommended";
            else if (averageRating >= 2.0)
                return "Average";
            else
                return "Not recommended";
        }
    }
}
