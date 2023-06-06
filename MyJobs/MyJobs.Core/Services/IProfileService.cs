namespace MyJobs.Core.Services
{
    using MyJobs.Core.Models.Profile;
    public interface IProfileService
    {
        UserProfileViewModel GetUserById(string id, string role);
    }
}
