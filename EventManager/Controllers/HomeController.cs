using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EventManagementSystem.Data;
using EventManagementSystem.Services.Contracts;


namespace EventManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404)
                {
                    return View("NotFound");
                }
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}