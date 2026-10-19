namespace HireGameDev.Domain.Entities;

/// <summary>
/// The category names of the skills master list (spec §17). Kept as strings rather than an
/// enum because the list is seeded data an admin will edit later; these constants exist so
/// code that treats one category specially (studios pick engines and genres) spells it once.
/// </summary>
public static class SkillCategory
{
    public const string Engines = "Engines";
    public const string Languages = "Languages";
    public const string ArtTools = "Art Tools";
    public const string DesignAndAudioTools = "Design & Audio Tools";
    public const string DevOpsAndInfrastructure = "DevOps & Infrastructure";
    public const string Genres = "Genres";
    public const string Platforms = "Platforms";
    public const string Roles = "Roles";
}
