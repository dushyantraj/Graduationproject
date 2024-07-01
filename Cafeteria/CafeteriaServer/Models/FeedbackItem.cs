namespace CafeteriaServer.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string FoodItem { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
         public DateTime CreatedAt { get; set; }
    }
}