using System;
using System.Collections.Generic;

namespace CafeteriaServer.Utilities

{


    public class EmployeeProfileValidator
    {
        public bool IsValidPreference(string preference)
        {
            string[] validPreferences = { "Vegetarian", "Non Vegetarian", "Eggetarian" };
            return Array.Exists(validPreferences, p => p.Equals(preference, StringComparison.OrdinalIgnoreCase));
        }
    }


}