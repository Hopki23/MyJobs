namespace MyJobs.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    
    using MyJobs.Infrastructure.Constants;
    [Authorize(Roles = RoleConstants.Administrator)]
    [Area("Admin")]
    public class BaseController : Controller
    {
    }
}
