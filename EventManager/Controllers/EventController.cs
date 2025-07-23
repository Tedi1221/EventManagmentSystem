using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventManagementSystem.Services.Contracts;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace EventManagementSystem.Controllers
{
    [Authorize] // Защита на всички действия в контролера с роля
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Преглед на събития с възможност за филтриране и странициране
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int page = 1)
        {
            const int pageSize = 6;
            var (events, totalCount) = await _eventService.GetAllAsync(searchTerm, categoryId, page, pageSize);

            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", categoryId);

            return View(events);
        }

        // Детайли на събитие
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        // Създаване на събитие (GET)
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var model = new EventFormViewModel
            {
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name")
            };
            return View(model);
        }

        // Създаване на събитие (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }
            await _eventService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // Редактиране на събитие (GET)
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

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
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", eventModel.CategoryId)
            };

            return View(viewModel);
        }

        // Редактиране на събитие (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, EventFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            await _eventService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // Изтриване на събитие (GET)
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        // Изтриване на събитие (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
