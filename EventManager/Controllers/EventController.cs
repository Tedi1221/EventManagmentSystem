// Път: Areas/Admin/Controllers/EventController.cs
using EventManagementSystem.Models; // <-- ТАЗИ ДИРЕКТИВА Е КЛЮЧОВА
using EventManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace EventManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: Admin/Event
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

        // GET: Admin/Event/Create
        public async Task<IActionResult> Create()
        {
            var model = new EventFormViewModel
            {
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name")
            };
            return View(model);
        }

        // POST: Admin/Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Admin/Event/Edit/5
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
                ExistingImageUrl = eventModel.ImageUrl,
                Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", eventModel.CategoryId)
            };

            return View(viewModel);
        }

        // POST: Admin/Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Admin/Event/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var eventModel = await _eventService.GetByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        // POST: Admin/Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
