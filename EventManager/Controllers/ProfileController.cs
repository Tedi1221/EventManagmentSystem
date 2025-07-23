// Път: Controllers/ProfileController.cs

using EventManagementSystem.Data;  // Добавете това за импортиране на ApplicationDbContext
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Models;




namespace EventManagementSystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }

        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string username, string email)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.UserName = username;
                user.Email = email;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
