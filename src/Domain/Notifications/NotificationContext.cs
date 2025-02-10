namespace Domain.Notifications;

public class NotificationContext
{
    private readonly List<Notification> _notifications = new List<Notification>();

    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    public bool HasNotifications => _notifications.Any();

    public void AddNotification(string message)
    {
        _notifications.Add(new Notification(message));
    }

    public void AddNotifications(IEnumerable<Notification> notifications)
    {
        _notifications.AddRange(notifications);
    }

    public void Clear()
    {
        _notifications.Clear();
    }
}
