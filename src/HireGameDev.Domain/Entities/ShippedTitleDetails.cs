namespace HireGameDev.Domain.Entities;

/// <summary>What a candidate enters for one shipped title; validated when it becomes a <see cref="ShippedTitle"/>.</summary>
public sealed record ShippedTitleDetails(
    string GameName,
    string StudioName,
    string Role,
    int Year);
