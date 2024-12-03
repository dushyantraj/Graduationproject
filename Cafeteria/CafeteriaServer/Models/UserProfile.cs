using System;

namespace CafeteriaServer.Models
{
    public class UserProfile
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string Preference { get; set; }
        public string SpiceLevel { get; set; }
        public string CuisinePreference { get; set; }
        public bool SweetTooth { get; set; }
    }
}