using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminEventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly UserManager<User> _userManager;

        public AdminEventController(IEventService eventService, UserManager<User> userManager)
        {
            _eventService = eventService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int page = 1)
        {
            const int pageSize = 10;
            var (events, totalCount) = await _eventService.GetAllAsync(searchTerm, categoryId, page, pageSize);

            ViewBag.TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", categoryId);

            return View(events);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Създаваме модела и му подаваме списъка с категории.
            var model = new EventFormViewModel
            {
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name")
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ако има грешка, зареждаме категориите отново преди да върнем изгледа.
                model.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            var eventToCreate = new Event
            {
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Price = model.Price,
                MaxParticipants = model.MaxParticipants,
                CategoryId = model.CategoryId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await _eventService.CreateAsync(eventToCreate, model.ImageFile);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null) return NotFound();

            var viewModel = new EventFormViewModel
            {
                Id = eventModel.Id,
                Name = eventModel.Name,
                Description = eventModel.Description,
                Date = eventModel.Date,
                Location = eventModel.Location,
                Price = eventModel.Price,
                MaxParticipants = eventModel.MaxParticipants,
                CategoryId = eventModel.CategoryId,
                ExistingImageUrl = eventModel.ImageUrl,
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", eventModel.CategoryId)
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            var eventToUpdate = await _eventService.GetByIdAsync(id);
            if (eventToUpdate == null) return NotFound();

            eventToUpdate.Name = model.Name;
            eventToUpdate.Description = model.Description;
            eventToUpdate.Date = model.Date;
            eventToUpdate.Location = model.Location;
            eventToUpdate.Price = model.Price;
            eventToUpdate.MaxParticipants = model.MaxParticipants;
            eventToUpdate.CategoryId = model.CategoryId;

            await _eventService.UpdateAsync(eventToUpdate, model.ImageFile);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _eventService.DeleteAsync(id, userId, true);

            return RedirectToAction(nameof(Index));
        }
    }
}