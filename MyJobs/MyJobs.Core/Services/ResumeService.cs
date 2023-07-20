namespace MyJobs.Core.Services
{
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
    }
}
