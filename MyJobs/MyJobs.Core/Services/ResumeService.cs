namespace MyJobs.Core.Services
{
    using iText.Kernel.Pdf;
    using iText.Layout;
    using iText.Layout.Properties;
    using iText.Layout.Element;
    using iText.IO.Image;

    using MyJobs.Core.Repositories;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    public class ResumeService : IResumeService
    {
        private readonly IDbRepository repository;

        public ResumeService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public byte[] GenerateResumePDF(ResumeViewModel model)
        {
            using MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = new(memoryStream);
            PdfDocument pdfDocument = new(writer);
            Document document = new(pdfDocument);

            if (!string.IsNullOrEmpty(model.Image))
            {
                byte[] photoBytes = Convert.FromBase64String(model.Image);
                Image image = new(ImageDataFactory.Create(photoBytes));
                image.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(image);
            }

            document.Add(new Paragraph("Title: " + model.Title));
            document.Add(new Paragraph("Summary: " + model.Summary));
            document.Add(new Paragraph("Education: " + model.Education));
            document.Add(new Paragraph("Experience: " + model.Experience));
            document.Add(new Paragraph("Address: " + model.Address));
            document.Add(new Paragraph("Phone Number: " + model.PhoneNumber));
            document.Add(new Paragraph("Date of Birth: " + model.DateOfBirth.ToShortDateString()));
            document.Add(new Paragraph("Gender: " + model.Gender));
            document.Add(new Paragraph("Skills: " + model.Skills));

            document.Close();

            return memoryStream.ToArray();
        }

        public async Task SaveResume(ResumeViewModel model, int employeeId)
        {
            var resume = new CV
            {
                EmployeeId = employeeId,
                Title = model.Title,
                Summary = model.Summary,
                Education = model.Education,
                Experience = model.Experience,
                Address = model.Address,
                Skills = model.Skills,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Image = model.Image,
                PhoneNumber = model.PhoneNumber
            };

            await this.repository.AddAsync(resume);
            await this.repository.SaveChangesAsync();
        }
    }
}
