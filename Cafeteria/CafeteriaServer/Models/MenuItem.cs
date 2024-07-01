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
namespace CafeteriaServer.Models
{
    public class MenuItem
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string FoodType { get; set; } // Ensure this property is defined
    }
}
