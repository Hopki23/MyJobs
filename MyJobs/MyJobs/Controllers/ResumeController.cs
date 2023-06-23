namespace MyJobs.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;

    public class ResumeController : Controller
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
        [Authorize(Roles = RoleConstants.Employee)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Employee)]
        public async Task<IActionResult> Create(ResumeViewModel model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    model.Image = Convert.ToBase64String(imageBytes);
                }

                var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var employee = await this.repository.All<Employee>()
                                .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    throw new ArgumentException("The requested employee was not found.");
                }

                await this.resumeService.SaveResume(model, employee.Id);

                byte[] resume = this.resumeService.GenerateResumePDF(model);

                return File(resume, "application/pdf", $"{employee.FirstName}_{employee.LastName}.pdf");
            }
            catch (Exception)
            {
                return View("CustomError");
            }
        }
    }
}
