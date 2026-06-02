namespace SFA.DAS.LearnerNotifications.Models
{
    public enum NotificationStatus: byte
    {
        Unread = 1,
        Acknowledged = 2,
        Hidden = 3,
        Expired = 4
    }
}
