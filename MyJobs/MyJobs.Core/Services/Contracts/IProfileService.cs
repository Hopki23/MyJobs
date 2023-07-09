namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Notification;
    using MyJobs.Core.Models.Profile;

    public interface IProfileService
    {
        Task<UserProfileViewModel> GetUserById(string id, string role);

        Task<IEnumerable<NotificationViewModel>> GetUnreadNotifications(string userId);
        Task<IEnumerable<NotificationViewModel>> GetReadNotifications(string userId);

        Task MarkNotificationAsRead(int id);

        Task<UserProfileViewModel> GetProfileForEditing(int id, string userId);

        Task EditProfile(UserProfileViewModel model, int id, string userId);
    }
}
