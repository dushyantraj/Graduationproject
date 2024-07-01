namespace CafeteriaServer.Models.DTO
{
    public class FeedbackDTO
    {
        public int Rating { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}