namespace CafeteriaServer.Models.DTO
{
    public class MenuItemDTO
    {
        public int ItemId { get; set; }
        public ItemDTO ItemDTO { get; set; }
        public double AverageRating { get; set; }
        public string OverallSentiment { get; set; }
        public string Recommendation {  get; set; }
    }
}
