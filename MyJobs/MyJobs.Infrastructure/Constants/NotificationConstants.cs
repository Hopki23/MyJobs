namespace MyJobs.Infrastructure.Constants
{
    public class NotificationConstants
    {
        public const string ErrorMessage = "ErrorMessage";
        public const string WarningMessage = "WarnMessage";
        public const string InformationMessage = "InfoMessage";
        public const string SuccessMessage = "SuccessMessage";

        public const string AlreadyApprovedMessageError = "You have already been approved for this job!";
        public const string AlreadyAppliedMessageError = "You have already applied for this job!";
        public const string SuccessApply = "You have successfully applied for the job!";
        public const string CreateResumeError = "You have to create your resume first!";

        public const string NotExistingJob = "The requested job was not found.";
        public const string NotExistingCategory = "Category does not exist!";
        public const string NotExistingResume = "The requested resume was not found.";
    }
}
