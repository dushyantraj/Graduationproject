public class ProfileDetails
{
    public string Preference { get; }
    public string SpiceLevel { get; }
    public string CuisinePreference { get; }
    public bool SweetTooth { get; }

    public ProfileDetails(string preference, string spiceLevel, string cuisinePreference, bool sweetTooth)
    {
        Preference = preference;
        SpiceLevel = spiceLevel;
        CuisinePreference = cuisinePreference;
        SweetTooth = sweetTooth;
    }
}