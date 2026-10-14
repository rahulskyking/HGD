namespace HireGameDev.Domain.Entities;

public class Notification : BaseEntity
{
    private Notification()
    {
    }

    public Notification(
        Guid userId,
        string type,
        string title,
        string message)
    {
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
    }

    public Guid UserId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public bool IsRead { get; private set; }
}