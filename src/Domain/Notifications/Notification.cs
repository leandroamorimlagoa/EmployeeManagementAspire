namespace Domain.Notifications;

public class Notification
{
    public DateTime DateTime { get; private set; }
    public string Message { get; private set; }
    public Notification(string message)
    {
        this.DateTime = DateTime.UtcNow;
        Message = message;
    }
}