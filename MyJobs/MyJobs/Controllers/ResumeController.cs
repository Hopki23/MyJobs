namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services;
    using MyJobs.Infrastructure.Models;

    public class ResumeController : BaseController
    {
        private readonly IResumeService resumeService;
        private readonly IDbRepository repository;

        public ResumeController(
            IResumeService resume,
            IDbRepository repository)
        {
            this.resumeService = resume;
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResumeViewModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                model.Image = Convert.ToBase64String(imageBytes);
            }

            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var employee = this.repository.All<Employee>()
                            .FirstOrDefault(e => e.UserId == userId);

            if (employee == null)
            {
                // Handle the case when the employee is not found
                return NotFound();
            }

            await this.resumeService.SaveResume(model, employee.EmployeeId);

            byte[] resume = this.resumeService.GenerateResumePDF(model);

            return File(resume, "application/pdf", $"{employee.FirstName}_{employee.LastName}.pdf");
        }
    }
}
