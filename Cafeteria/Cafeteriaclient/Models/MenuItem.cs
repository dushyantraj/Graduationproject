namespace CafeteriaClient.Models
{
    public class MenuItem
    {
        public string MenuType { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Availability { get; set; }
        public string FoodType { get; set; }
        public string CuisinePreference { get; set; }
        public string SpiceLevel { get; set; }
    }

}
