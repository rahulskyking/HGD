namespace HireGameDev.Domain;

/// <summary>
/// A supported Indian hiring hub. Studio profiles select a city from this list so city/state
/// stay consistent for job filters; candidate profiles still accept free text with suggestions.
/// Add missing cities here when studios need them.
/// </summary>
public sealed record IndianCity(string Name, string State)
{
    public static IReadOnlyList<IndianCity> All { get; } =
    [
        new("Ahmedabad", "Gujarat"),
        new("Amritsar", "Punjab"),
        new("Bengaluru", "Karnataka"),
        new("Bhopal", "Madhya Pradesh"),
        new("Bhubaneswar", "Odisha"),
        new("Chandigarh", "Chandigarh"),
        new("Chennai", "Tamil Nadu"),
        new("Coimbatore", "Tamil Nadu"),
        new("Dehradun", "Uttarakhand"),
        new("Delhi", "Delhi"),
        new("Gandhinagar", "Gujarat"),
        new("Gurugram", "Haryana"),
        new("Guwahati", "Assam"),
        new("Hyderabad", "Telangana"),
        new("Indore", "Madhya Pradesh"),
        new("Jaipur", "Rajasthan"),
        new("Kochi", "Kerala"),
        new("Kolkata", "West Bengal"),
        new("Lucknow", "Uttar Pradesh"),
        new("Ludhiana", "Punjab"),
        new("Mangaluru", "Karnataka"),
        new("Mumbai", "Maharashtra"),
        new("Mysuru", "Karnataka"),
        new("Nagpur", "Maharashtra"),
        new("Nashik", "Maharashtra"),
        new("Noida", "Uttar Pradesh"),
        new("Panaji", "Goa"),
        new("Patna", "Bihar"),
        new("Pune", "Maharashtra"),
        new("Raipur", "Chhattisgarh"),
        new("Ranchi", "Jharkhand"),
        new("Surat", "Gujarat"),
        new("Thiruvananthapuram", "Kerala"),
        new("Udaipur", "Rajasthan"),
        new("Vadodara", "Gujarat"),
        new("Varanasi", "Uttar Pradesh"),
        new("Visakhapatnam", "Andhra Pradesh"),
    ];

    public static IndianCity? Find(string? name) =>
        name is null
            ? null
            : All.FirstOrDefault(city => string.Equals(city.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
}
