namespace MyJobs.Core.Models.Resume
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class UploadResumeViewModel
    {
        [Required(ErrorMessage = "Please select your resume file.")]
        [Display(Name = "Resume File")]
        public IFormFile ResumeFile { get; set; } = null!;
        public List<int> JobIds { get; set; } = new List<int>();
        public int Id { get; set; }
    }
}
