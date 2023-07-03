namespace MyJobs.Core.Models.Notification
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public int NotificationsCount { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }

        //public DateTime SentOn { get; set; }
        public bool IsRead { get; set; }
    }
}
