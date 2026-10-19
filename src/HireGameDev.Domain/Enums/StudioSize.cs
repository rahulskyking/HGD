namespace HireGameDev.Domain.Enums;

/// <summary>Head-count band of a studio (spec §7.3). Stored as its number.</summary>
public enum StudioSize
{
    UpTo10 = 1,
    From11To50 = 2,
    From51To200 = 3,
    Over200 = 4,
}
