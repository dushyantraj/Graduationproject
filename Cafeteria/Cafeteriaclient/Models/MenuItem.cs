namespace CafeteriaClient.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string MenuType { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
