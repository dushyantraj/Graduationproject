namespace CafeteriaServer.Models.DTO
{
    public class AddMenuItemDTO
    {
        public string MenuType { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string FoodType { get; set; }
        public string CuisinePreference { get; set; }
        public string SpiceLevel { get; set; }
    }
}