namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Models.Resume;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Models;

    public class JobService : IJobService
    {
        private readonly IDbRepository repository;
        private readonly IGetCategoriesService categoriesService;

        public JobService(
            IDbRepository repository,
            IGetCategoriesService categoriesService)
        {
            this.repository = repository;
            this.categoriesService = categoriesService;
        }

        public async Task Apply(UploadResumeViewModel model, Employee employee)
        {
            int jobId = model.Id;

            var resume = await this.repository.All<CV>()
                .FirstOrDefaultAsync(c => c.EmployeeId == employee.Id);

            if (resume == null)
            {
                throw new ArgumentException("The requested resume was not found.");
            }

            var job = await this.repository.GetByIdAsync<Job>(jobId);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            resume.Jobs.Add(job);
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

        public async Task Delete(int id)
        {
            var job = this.repository.All<Job>()
                .FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            job!.IsDeleted = true;
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<Job> FilterJobOffers(string select, string[] selectedWorkingTimes, string locationSelect)
        {
            IQueryable<Job> filteredJobOffers = this.repository.AllReadonly<Job>();

            if (!string.IsNullOrEmpty(select))
            {
                filteredJobOffers = filteredJobOffers.Where(j => j.Category.Id.ToString() == select);
            }

            if (selectedWorkingTimes != null && selectedWorkingTimes.Length > 0)
            {
                filteredJobOffers = filteredJobOffers.Where(j => selectedWorkingTimes.Contains(j.WorkingTime));
            }

            if (!string.IsNullOrEmpty(locationSelect))
            {
                filteredJobOffers = filteredJobOffers.Where(j => j.Town == locationSelect);
            }

            filteredJobOffers = filteredJobOffers.Include(j => j.Category);

            return filteredJobOffers.ToList();
        }

        public IEnumerable<JobsViewModel> GetAllJobs(int page, int itemsToTake)
        {
            var jobs = this.repository.AllReadonly<Job>()
                 .Where(x => x.IsDeleted == false)
                 .OrderByDescending(j => j.CreatedOn)
                 .Skip((page - 1) * itemsToTake)
                 .Take(itemsToTake)
                 .Select(j => new JobsViewModel
                 {
                     Id = j.Id,
                     Title = j.Title,
                     CategoryName = j.Category.Name,
                     CategoryId = j.CategoryId,
                     
                 })
                 .ToList();

            return jobs;
        }

        public EditJobViewModel GetById(int id, string userId)
        {
            var job =  this.repository.AllReadonly<Job>()
                .Where(j => j.Id == id && j.Employer.UserId == userId)
                .Select(j => new EditJobViewModel
                {
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Responsibilities = j.Responsibilities,
                    TownName = j.Town,
                    WorkingTime = j.WorkingTime,
                    Salary = j.Salary,
                    Offering = j.Offering
                })
                .FirstOrDefault();

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            return job;
        }

        public JobFilterViewModel GetJobFilterViewModel()
        {
            var categories = this.categoriesService.GetAllCategories();

            var workingTimes = this.repository
                .AllReadonly<Job>()
                .Select(j => j.WorkingTime)
                .Distinct()
                .ToList();
            var townNames = this.repository
                .AllReadonly<Job>()
                .Select(j => j.Town)
                .Distinct()
                .ToList();

            var jobFilterModel = new JobFilterViewModel()
            {
                Categories = categories.ToList(),
                WorkingTimes = workingTimes!,
                TownNames = townNames
            };

            return jobFilterModel;
        }

        public async Task<IEnumerable<Job>> GetJobsByEmployeeId(int employeeId)
        {
            return await this.repository.AllReadonly<Job>()
                 .Where(j => j.Resumes.Any(r => r.EmployeeId == employeeId))
                 .Include(j => j.Category)
                 .Include(j => j.Company)
                 .ToListAsync();
        }

        public async Task<IEnumerable<JobsViewModel>> GetJobsForCertainEmployer(Employer employer)
        {
            return await this.repository.AllReadonly<Job>()
            .Where(j => j.EmployerId == employer.Id)
            .Select(x => new JobsViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CategoryName = x.Category.Name
            })
            .ToListAsync();
        }

        public IEnumerable<JobsWithCVsViewModel> GetJobsWithCV(JobsWithCVsViewModel model, Employer employer)
        {
            var jobs = this.repository.AllReadonly<Job>()
            .Where(x => x.EmployerId == employer.Id)
            .ToList();

            var jobViewModels = new List<JobsWithCVsViewModel>();

            foreach (var job in jobs)
            {
                var cvs = this.repository.AllReadonly<CV>()
                    .Where(c => c.Jobs.Contains(job) && !c.IsDeleted)
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
            var job = this.repository.AllReadonly<Job>()
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
                    IsOwner = (employer != null && j.EmployerId == employer.Id)
                })
                .FirstOrDefault();

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            return job;
        }

        public int GetTotalJobCount()
        {
            return this.repository.AllReadonly<Job>().Count();
        }

        public async Task Update(int id, EditJobViewModel model)
        {
            var job = this.repository.All<Job>()
                .FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            job.Title = model.Title;
            job.Description = model.Description;
            job.Requirements = model.Requirements;
            job.Responsibilities = model.Responsibilities;
            job.Town = model.TownName;
            job.WorkingTime = model.WorkingTime;
            job.Salary = model.Salary;
            job.Offering = model.Offering;
            job.CategoryId = model.CategoryId;

            await this.repository.SaveChangesAsync();
        }
    }
}