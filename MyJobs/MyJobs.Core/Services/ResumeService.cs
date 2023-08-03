namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
   
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Properties;
    using iText.Layout.Element;
    using iText.IO.Image;
    using iText.Kernel.Font;
    using iText.IO.Font.Constants;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;
    using MyJobs.Core.Services.Contracts;

    public class ResumeService : IResumeService
    {
        private readonly IDbRepository repository;

        public ResumeService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task<FileContentResult> DownloadResume(string userId, int cvId)
        {
            var employee = await this.repository.AllReadonly<Employee>()
               .Where(e => e.UserId == userId).FirstOrDefaultAsync();

            var cv = await this.repository.AllReadonly<CV>()
                .FirstOrDefaultAsync(r => r.Id == cvId && r.EmployeeId == employee!.Id);

            // Set the file name for the download
            string fileName = cv.ResumeFileName;

            // Return the PDF file data as a downloadable file response
            return new FileContentResult(cv.ResumeFile, "application/pdf") { FileDownloadName = fileName };
        }

        public async Task<EditResumeViewModel> GetResumeForEdit(string userId, int id)
        {
            var resume = await this.repository.AllReadonly<CV>()
                .Where(j => j.Id == id && j.Employee.UserId == userId)
                .Select(j => new EditResumeViewModel
                {
                    Id = j.Id,
                    Image = j.Image,
                    Title = j.Title,
                    Summary = j.Summary,
                    DateOfBirth = j.DateOfBirth,
                    Gender = j.Gender,
                    Education = j.Education,
                    Experience = j.Experience,
                    Skills = j.Skills,
                    Address = j.Address,
                    PhoneNumber = j.PhoneNumber
                })
                .FirstOrDefaultAsync();

            if (resume == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            return resume;
        }

        public byte[] GenerateResumePDF(ResumeViewModel model)
        {
            using MemoryStream memoryStream = new();
            using PdfWriter writer = new(memoryStream);
            using PdfDocument pdfDocument = new(writer);
            using Document document = new(pdfDocument);

            PdfFont sectionFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);


            if (!string.IsNullOrEmpty(model.Image))
            {
                byte[] photoBytes = Convert.FromBase64String(model.Image);
                Image image = new(ImageDataFactory.Create(photoBytes));
                image.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(image);
            }

            Dictionary<string, string> resumeContent = new()
                {
                    { "First Name", model.FirstName },
                    { "Last Name", model.LastName },
                    { "Title", model.Title },
                    { "Summary", model.Summary },
                    { "Education", model.Education },
                    { "Experience", model.Experience },
                    { "Address", model.Address },
                    { "Phone Number", model.PhoneNumber },
                    { "Date of Birth", model.DateOfBirth.ToShortDateString() },
                    { "Gender", model.Gender },
                    { "Skills", model.Skills }
                };

            document.Add(new Paragraph("Resume")
                .SetFont(sectionFont)
                .SetFontSize(16)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));

            foreach (var item in resumeContent)
            {
                document.Add(new Paragraph($"{item.Key}: {item.Value}"));
            }

            document.Close();

            return memoryStream.ToArray();
        }

        public async Task<IEnumerable<CV>> GetUserResumes(string userId)
        {
            var employee = await this.repository.AllReadonly<Employee>()
                .Where(e => e.UserId == userId).FirstOrDefaultAsync();

            return await this.repository.AllReadonly<CV>()
                .Include(c => c.Employee)
                .Where(r => r.EmployeeId == employee!.Id && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task SaveResume(ResumeViewModel model, int employeeId)
        {
            var employee = await this.repository.GetByIdAsync<Employee>(employeeId);

            if (employee == null)
            {
                throw new ArgumentException("Invalid employee");
            }

            string fileName = $"{employee.FirstName}_{employee.LastName}.pdf";

            var resume = new CV
            {
                Employee = employee,
                Title = model.Title,
                Summary = model.Summary,
                Education = model.Education,
                Experience = model.Experience,
                Address = model.Address,
                Skills = model.Skills,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Image = model.Image,
                PhoneNumber = model.PhoneNumber,
                ResumeFileName = fileName
            };

            byte[] resumeBytes = GenerateResumePDF(model);
            resume.ResumeFile = resumeBytes;

            await this.repository.AddAsync(resume);
            await this.repository.SaveChangesAsync();
        }

        public async Task Update(int id, EditResumeViewModel model)
        {
            var resume = await this.repository.All<CV>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (resume == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            if (!string.IsNullOrEmpty(model.Image))
            {
                resume.Image = model.Image;
            }
            else if (model.IsPictureRemoved)
            {
                resume.Image = null;
            }

            resume.Title = model.Title;
            resume.Summary = model.Summary;
            resume.DateOfBirth = model.DateOfBirth;
            resume.Gender = model.Gender;
            resume.Education = model.Education;
            resume.Experience = model.Experience;
            resume.Skills = model.Skills;
            resume.Address = model.Address;
            resume.PhoneNumber = model.PhoneNumber;

            await this.repository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var resume = await this.repository.All<CV>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (resume == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            resume.IsDeleted = true;
            await this.repository.SaveChangesAsync();
        }
    }
}
