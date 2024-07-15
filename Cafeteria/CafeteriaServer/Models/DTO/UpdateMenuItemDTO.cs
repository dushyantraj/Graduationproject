namespace CafeteriaServer.Models.DTO
{
    public class UpdateMenuItemDTO
    {
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
        public string SpiceLevel { get; set; }
    }
}