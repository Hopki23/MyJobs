﻿using Microsoft.AspNetCore.Mvc;

namespace MyJobs.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
