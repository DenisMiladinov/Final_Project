using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Services.Services;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FAQs() => View();
        public IActionResult Terms() => View();
        public IActionResult PrivacyPolicy() => View();
        public IActionResult Contact() => View();
    }
}
