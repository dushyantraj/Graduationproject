namespace CafeteriaServer.Models.DTO
{
    public class FeedbackDTO
    {
        public double Rating { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}