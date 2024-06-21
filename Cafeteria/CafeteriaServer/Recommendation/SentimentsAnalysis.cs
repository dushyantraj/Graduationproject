using System;

namespace CafeteriaServer.Recommendation
{
    public static class SentimentsAnalysis
    {
        public static (double AverageRating, string OverallSentiment, string Recommendation) AnalyzeSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            var overallMetrics = CalculateOverallSentimentsAndRatings(entries);
            return overallMetrics;
        }
        public static (double AverageRating, string OverallSentiment, string Recommendation) CalculateOverallSentimentsAndRatings(List<(double Rating, string Comment, DateTime CreatedAt)> entries)
        {
            int positiveCount = 0;
            int negativeCount = 0;
            int neutralCount = 0;

            double totalRating = 0;

            foreach (var entry in entries)
            {
                string comment = entry.Comment.ToLower();
                int sentimentScore = 0;
                bool isNegated = false;


                string[] words = comment.Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    if (SentimentWords.NegationWords.Contains(word))
                    {
                        isNegated = true;
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

                if (sentimentScore > 0)
                {
                    positiveCount++;
                }
                else if (sentimentScore < 0)
                {
                    negativeCount++;
                }
                else
                {
                    neutralCount++;
                }

                totalRating += entry.Rating;
            }

            double averageRating = entries.Count > 0 ? totalRating / entries.Count : 0.0;

            string overallSentiment = DetermineOverallSentiment(positiveCount, negativeCount, neutralCount);

            string recommendation = averageRating >= 4.0 ? "Highly recommended" :
                                    averageRating >= 3.0 ? "Recommended" :
                                    averageRating >= 2.0 ? "Average" :
                                    "Not recommended";

            return (averageRating, overallSentiment, recommendation);
        }

        private static string DetermineOverallSentiment(int positiveCount, int negativeCount, int neutralCount)
        {
            if (positiveCount > negativeCount)
            {
                return "Positive";
            }
            else if (negativeCount > positiveCount)
            {
                return "Negative";
            }
            else
            {
                return "Neutral";
            }
        }
        public static string CalculateOverallSentiment(int positiveCount, int negativeCount, int totalFeedback)
        {
            if (totalFeedback == 0) return "N/A";

            if (positiveCount > negativeCount)
            {
                return "Positive";
            }
            else if (negativeCount > positiveCount)
            {
                return "Negative";
            }
            else
            {
                return "Neutral";
            }
        }

    }
}