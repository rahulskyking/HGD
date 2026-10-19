namespace HireGameDev.Domain.Enums;

/// <summary>What kind of company a studio account is (spec §7.3). Stored as its number.</summary>
public enum StudioType
{
    GameStudio = 1,
    Outsourcing = 2,
    Publisher = 3,
    Indie = 4,
}
