﻿namespace MyJobs.Core.Services.Contracts
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using MyJobs.Core.Models.Resume;
    using MyJobs.Infrastructure.Models;

    public interface IResumeService
    {
        Task SaveResume(ResumeViewModel model, int employeeId);
        byte[] GenerateResumePDF(ResumeViewModel model);
        Task<IEnumerable<CV>> GetUserResumes(string userId);
        Task<EditResumeViewModel> GetResumeForEdit(string userId, int id);
        Task<FileContentResult> DownloadResume(string userId, int cvId);
        Task Update(int id, EditResumeViewModel model, IFormFile? updatePicture);
        Task Delete(int id);
    }
}
