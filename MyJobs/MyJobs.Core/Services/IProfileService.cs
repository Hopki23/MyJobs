namespace MyJobs.Core.Services
{
    using MyJobs.Core.Models.Profile;
    using MyJobs.Infrastructure.Data.Models;

    public interface IProfileService
    {
        UserProfileViewModel GetUserById(string id, string role);

        Task<IEnumerable<Notification>> GetUnreadNotificationsForEmployee(int employeeId);
        Task MarkNotificationAsRead(int notificationId);
    }
}
