namespace HireGameDev.Domain.Entities;

/// <summary>What a candidate enters for one portfolio link; validated when it becomes a <see cref="PortfolioLink"/>.</summary>
public sealed record PortfolioLinkDetails(
    string Url,
    string? Label);
