namespace HireGameDev.Domain.Entities;

public class Payment : BaseEntity
{
    private Payment()
    {
    }

    public Payment(
        Guid userId,
        string type,
        decimal amount)
    {
        UserId = userId;
        Type = type;
        Amount = amount;
        Status = "Pending";
    }

    public Guid UserId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public string Status { get; private set; } = string.Empty;

    public string? RazorpayPaymentId { get; private set; }

    public decimal? GstAmount { get; private set; }

    public DateTime? BillingCycleEndUtc { get; private set; }
}