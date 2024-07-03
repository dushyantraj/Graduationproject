namespace CafeteriaServer.Models
{
    public class MenuItem
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string FoodType { get; set; }
    }
}
