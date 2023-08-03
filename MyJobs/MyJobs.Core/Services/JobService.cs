namespace MyJobs.Core.Services
{
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;

    using MyJobs.Core.Models.Job;
    using MyJobs.Core.Repositories;
    using MyJobs.Core.Services.Contracts;
    using MyJobs.Infrastructure.Constants;
    using MyJobs.Infrastructure.Models;

    public class JobService : IJobService
    {
        private readonly IDbRepository repository;
        private readonly ICategoryService categoriesService;

        public JobService(
            IDbRepository repository,
            ICategoryService categoriesService)
        {
            this.repository = repository;
            this.categoriesService = categoriesService;
        }

        public async Task Apply(int resumeId, string userId, int jobId)
        {
            var employee = await this.repository.All<Employee>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            var resume = await this.repository.All<CV>()
                .FirstOrDefaultAsync(c => c.Id == resumeId && c.EmployeeId == employee!.Id);

            //if user tries to apply with wrong resume(not created via the application or does not have resume yet)
            //if (resume == null)
            //{
            //    throw new ArgumentException(NotificationConstants.CreateResumeError);
            //}

            var job = await this.repository.All<Job>()
                .Include(j => j.Resumes)
                .Include(e => e.Employees)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null || job.IsDeleted)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            //If user tries to apply if he had already applied
            if (job.Resumes.Any(x => x.EmployeeId == employee!.Id))
            {
                throw new InvalidOperationException(NotificationConstants.AlreadyAppliedMessageError);
            }

            //if user tries to apply if he's already approved for work
            if (job.Employees.Any(e => e.Id == employee!.Id))
            {
                throw new ArgumentException(NotificationConstants.AlreadyApprovedMessageError);
            }

            resume.Jobs.Add(job);
            job.Resumes.Add(resume);

            await this.repository.SaveChangesAsync();
        }

        public async Task CreateAsync(CreateJobViewModel model, string userId)
        {
            var employer = await this.repository.All<Employer>()
                .Where(e => e.UserId == userId)
                .Select(e => new { e.Id, e.CompanyId })
                .FirstOrDefaultAsync();

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
                EmployerId = employer!.Id,
                CompanyId = employer!.CompanyId,
                Salary = model.Salary,
                WorkingTime = model.WorkingTime,
                IsApproved = false
            };

            await this.repository.AddAsync(job);
            await this.repository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var job = await this.repository.All<Job>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            job.IsDeleted = true;
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<Job> FilterJobOffers(string select, string[] selectedWorkingTimes, string locationSelect)
        {
            var filteredJobOffers = this.repository.AllReadonly<Job>()
                .Where(x => !x.IsDeleted);

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

        public async Task<IEnumerable<JobsViewModel>> GetAllJobs(int page, int itemsToTake)
        {
            var jobs =  await this.repository.AllReadonly<Job>()
                 .Where(x => x.IsApproved && !x.IsDeleted)
                 .OrderByDescending(j => j.CreatedOn)
                 .Skip((page - 1) * itemsToTake)
                 .Take(itemsToTake)
                 .Select(j => new JobsViewModel
                 {
                     Id = j.Id,
                     Title = j.Title,
                     CategoryName = j.Category.Name,
                     CategoryId = j.CategoryId,
                     IsDeleted = j.IsDeleted
                 })
                 .ToListAsync();

            return jobs;
        }

        public async Task<IEnumerable<JobsViewModel>> GetAllJobs()
        {
            return await this.repository.AllReadonly<Job>()
                 .OrderByDescending(j => j.Id)
                 .Select(j => new JobsViewModel
                 {
                     Id = j.Id,
                     Title = j.Title,
                     CategoryName = j.Category.Name,
                     CategoryId = j.CategoryId,
                     IsApproved = j.IsApproved,
                     IsDeleted = j.IsDeleted
                 })
                 .ToListAsync();
        }

        public async Task<EditJobViewModel> GetById(int id, string userId)
        {
            var job = await this.repository.AllReadonly<Job>()
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
                .FirstOrDefaultAsync();

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            return job;
        }

        public async Task ApproveJob(int id)
        {
            var job = await this.repository.All<Job>()
                 .FirstOrDefaultAsync(x => x.Id == id);

            if (job == null)
            {
                throw new ArgumentException("Job not found!");
            }

            job.IsApproved = true;

            await this.repository.SaveChangesAsync();
        }

        public async Task<JobFilterViewModel> GetJobFilterViewModel()
        {
            var categories = await this.categoriesService.GetAllCategories();

            var workingTimes = await this.repository
                .AllReadonly<Job>()
                .Select(j => j.WorkingTime)
                .Distinct()
                .ToListAsync();
            var townNames = await this.repository
                .AllReadonly<Job>()
                .Select(j => j.Town)
                .Distinct()
                .ToListAsync();

            var jobFilterModel = new JobFilterViewModel()
            {
                Categories = categories.ToList(),
                WorkingTimes = workingTimes!,
                TownNames = townNames
            };

            return jobFilterModel;
        }

        public async Task<IEnumerable<Job>> GetJobsByEmployeeId(string userId)
        {
            var employee = await this.repository.AllReadonly<Employee>()
               .FirstOrDefaultAsync(e => e.UserId == userId);

            return await this.repository.AllReadonly<Job>()
                .Where(j => j.Resumes.Any(r => r.EmployeeId == employee!.Id))
                 .Include(j => j.Category)
                 .Include(j => j.Company)
                 .ToListAsync();
        }

        public async Task<IEnumerable<JobsViewModel>> GetJobsForCertainEmployer(string userId)
        {
            var employer = await this.repository.AllReadonly<Employer>()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return await this.repository.AllReadonly<Job>()
            .Where(j => j.EmployerId == employer!.Id)
            .Select(x => new JobsViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CategoryName = x.Category.Name,
                IsApproved = x.IsApproved
            })
            .OrderByDescending(x => x.Id)
            .ToListAsync();
        }

        public async Task<IEnumerable<JobsWithCVsViewModel>> GetJobsWithCV(JobsWithCVsViewModel model, string userId)
        {
            var employer = await this.repository.All<Employer>()
                           .FirstOrDefaultAsync(e => e.UserId == userId);

            var jobs = await this.repository.AllReadonly<Job>()
            .Where(x => x.EmployerId == employer!.Id)
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();

            var jobViewModels = new List<JobsWithCVsViewModel>();

            foreach (var job in jobs)
            {
                var cvs = await this.repository.AllReadonly<CV>()
                    .Include(c => c.Employee)
                    .Where(c => c.Jobs.Contains(job) && !c.IsDeleted)
                    .ToListAsync();

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

        //Returns single job details
        public async Task<SingleJobViewModel> GetSingleJob(int id, string userId)
        {
            var employer = await this.repository.All<Employer>()
                                        .FirstOrDefaultAsync(e => e.UserId == userId);

            var job = await this.repository.AllReadonly<Job>()
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
                .FirstOrDefaultAsync();

            if (job == null)
            {
                throw new ArgumentException("The requested job was not found.");
            }

            return job;
        }

        public async Task<int> GetTotalJobCount()
        {
            return await this.repository.AllReadonly<Job>()
                .Where(j => j.IsApproved && !j.IsDeleted)
                .CountAsync();
        }

        public async Task Update(int id, EditJobViewModel model)
        {
            var job = await this.repository.All<Job>()
                .FirstOrDefaultAsync(x => x.Id == id);

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