namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Infrastructure.Models;

    public class JobService : IJobService
    {
        private readonly IDbRepository repository;

        public JobService(IDbRepository repository)
        {
            this.repository = repository;
        }

        public async Task Apply(UploadResumeViewModel model, Employee employee)
        {
            int jobId = model.Id;

            var resume = this.repository.AllReadonly<CV>()
                .Where(c => c.EmployeeId == employee.EmployeeId)
                .FirstOrDefault();

            var job = await this.repository.GetByIdAsync<Job>(jobId);

            resume!.Jobs.Add(job);
            job.Resumes.Add(resume);

            await this.repository.SaveChangesAsync();
        }

        public async Task CreateAsync(CreateJobViewModel model, int employerId, int companyId)
        {
            var job = new Job
            {
                Title = model.Title,
                Town = model.TownName,
                CreatedOn = DateTime.UtcNow,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Requirements = model.Requirements,
                Responsibilities = model.Responsibilities,
                Offering = model.Offering,
                EmployerId = employerId,
                CompanyId = companyId,
                Salary = model.Salary,
                WorkingTime = model.WorkingTime
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<JobsViewModel> GetAllJobs(int page, int itemsToTake)
        {
            var jobs = this.repository.AllReadonly<Job>()
                 .OrderByDescending(j => j.CreatedOn)
                 .Skip((page - 1) * itemsToTake)
                 .Take(itemsToTake)
                 .Select(j => new JobsViewModel
                 {
                     Id = j.Id,
                     Title = j.Title,
                     CategoryName = j.Category.Name,
                     CategoryId = j.CategoryId
                 })
                 .ToList();

            return jobs;
        }

        public IEnumerable<JobsWithCVsViewModel> GetJobsWithCV(JobsWithCVsViewModel model, Employer employer)
        {
            var jobs = this.repository.AllReadonly<Job>()
            .Where(x => x.EmployerId == employer.EmployerId)
            .ToList();

            var jobViewModels = new List<JobsWithCVsViewModel>();

            foreach (var job in jobs)
            {
                var cvs = this.repository.AllReadonly<CV>()
                    .Where(c => c.Jobs.Contains(job))
                    .ToList();

                if (cvs.Count > 0)
                {
                    var jobViewModel = new JobsWithCVsViewModel
                    {
                        Job = job,
                        CVs = cvs
                    };

                    jobViewModels.Add(jobViewModel);
                }
            }

            return jobViewModels;
        }

        public SingleJobViewModel GetSingleJob(int id, Employer employer)
        {
            return this.repository.AllReadonly<Job>()
                .Where(j => j.Id == id)
                .Select(j => new SingleJobViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    Town = j.Town,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Offering = j.Offering,
                    CreatedOn = j.CreatedOn,
                    CompanyName = j.Company.CompanyName,
                    PhoneNumber = j.Company.PhoneNumber,
                    Address = j.Company.Address,
                    EmployerFirstName = j.Employer.FirstName,
                    EmployerLastName = j.Employer.LastName,
                    Category = j.Category.Name,
                    Responsibilities = j.Responsibilities,
                    Salary = j.Salary,
                    WorkingTime = j.WorkingTime,
                    IsOwner = (employer != null && j.EmployerId == employer.EmployerId)
                })
                .FirstOrDefault()!;
        }

        public int GetTotalJobCount()
        {
            return this.repository.AllReadonly<Job>().Count();
        }
    }
}
