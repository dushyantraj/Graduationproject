namespace CafeteriaServer.Models
{
    public class FeedbackItem
    {
        public int Id { get; set; }
        public string FoodItem { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
    }
}
