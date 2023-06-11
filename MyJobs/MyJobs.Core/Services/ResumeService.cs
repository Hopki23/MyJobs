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
            using MemoryStream memoryStream = new MemoryStream();
            using PdfWriter writer = new(memoryStream);
            using PdfDocument pdfDocument = new(writer);
            using Document document = new(pdfDocument);

            if (!string.IsNullOrEmpty(model.Image))
            {
                byte[] photoBytes = Convert.FromBase64String(model.Image);
                Image image = new(ImageDataFactory.Create(photoBytes));
                image.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(image);
            }

            document.Add(new Paragraph("First Name: " + model.FirstName));
            document.Add(new Paragraph("Last Name: " + model.LastName));
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
