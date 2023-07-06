namespace MyJobs.Core.Services.Contracts
{
    using MyJobs.Core.Models.Notification;
    using MyJobs.Core.Models.Profile;

    public interface IProfileService
    {
        Task<UserProfileViewModel> GetUserById(string id, string role);

        Task<IEnumerable<NotificationViewModel>> GetUnreadNotifications(int id);
        Task<IEnumerable<NotificationViewModel>> GetReadNotifications(int id);

        Task MarkNotificationAsRead(int id);

        Task<UserProfileViewModel> GetProfileForEditing(int id, string userId);

        Task EditProfile(UserProfileViewModel model, int id, string userId);
    }
}
