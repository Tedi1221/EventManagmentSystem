using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class EventController : Controller
{
    private readonly IEventService _eventService;
    private readonly UserManager<User> _userManager;

    public EventController(IEventService eventService, UserManager<User> userManager)
    {
        _eventService = eventService;
        _userManager = userManager;
    }

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

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var eventModel = await _eventService.GetByIdAsync(id);
        if (eventModel == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.CanEdit = currentUser != null && (await _userManager.IsInRoleAsync(currentUser, "Administrator"));

        return View(eventModel);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create()
    {
        var model = new EventFormViewModel
        {
            Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name")
        };
        return View(model);
    }

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
            Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", eventModel.CategoryId),
            ExistingImageUrl = eventModel.ImageUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id, EventFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        await _eventService.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _eventService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}