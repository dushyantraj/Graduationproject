namespace CafeteriaServer.Models.DTO
{
    public class ItemDTO
    {
        public int ItemId { get; set; }
        public int MenuId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string FoodType { get; set; }
    }
}
