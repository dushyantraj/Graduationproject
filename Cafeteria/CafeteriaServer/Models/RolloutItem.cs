using System;

namespace CafeteriaServer.Models
{
    // public class RolloutItem
    // {
    //     public int RolloutId { get; set; }
    //     public string ItemName { get; set; }
    //     public decimal Price { get; set; }
    //     public int Available { get; set; }
    //     public DateTime DateRolledOut { get; set; } // Example property for date rolled out

    //     // Additional properties and methods as needed
    // }
    public class RolloutItem
    {
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Available { get; set; }
    }
}
