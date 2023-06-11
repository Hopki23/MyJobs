namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Profile;
    using MyJobs.Infrastructure.Data.Models;

    public interface IProfileService
    {
        UserProfileViewModel GetUserById(string id, string role);

        Task<IEnumerable<Notification>> GetUnreadNotificationsForEmployee(int employeeId);

        Task<Notification> MarkNotificationAsRead(int notificationId);

        Task<UserProfileViewModel> GetProfileForEditing(int id, string userId);

        Task EditProfile(UserProfileViewModel model, int id, string userId);
    }
}
