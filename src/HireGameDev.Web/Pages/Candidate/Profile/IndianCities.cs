namespace HireGameDev.Web.Pages.Candidate.Profile;

/// <summary>
/// Suggestions for the City field (a datalist, so any city can still be typed). The game-dev
/// hubs from the spec come first, then other cities with studios.
/// </summary>
public static class IndianCities
{
    public static IReadOnlyList<string> Suggestions { get; } =
    [
        "Bengaluru", "Hyderabad", "Pune", "Mumbai", "Chennai", "Noida", "Kochi", "Ahmedabad",
        "Delhi", "Gurugram", "Kolkata", "Thiruvananthapuram", "Jaipur", "Chandigarh", "Indore",
        "Bhubaneswar", "Coimbatore", "Nagpur", "Lucknow", "Visakhapatnam", "Mysuru", "Vadodara",
        "Surat", "Mangaluru", "Panaji",
    ];
}
