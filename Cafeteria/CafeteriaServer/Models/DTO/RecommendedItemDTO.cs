namespace CafeteriaServer.Models.DTO
{
    public class RecommendedItemDTO
    {
        public int RolloutId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public double AverageRating { get; set; }
        public string OverallSentiment { get; set; }
        public string Recommendation { get; set; }
    }
}