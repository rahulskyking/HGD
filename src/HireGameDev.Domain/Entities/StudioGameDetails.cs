namespace HireGameDev.Domain.Entities;

/// <summary>The values a studio enters for one released game; the aggregate turns them into rows.</summary>
public sealed record StudioGameDetails(
    string Name,
    int Year,
    string? Url);
