using EventManagementSystem.Data; 
using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

[Authorize]
public class EventController : Controller
{
    private readonly IEventService _eventService;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context; 

    
    public EventController(IEventService eventService, UserManager<User> userManager, ApplicationDbContext context)
    {
        _eventService = eventService;
        _userManager = userManager;
        _context = context;
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
        if (currentUser != null)
        {
            ViewBag.CanEdit = (eventModel.UserId == currentUser.Id);
        }

        return View(eventModel);
    }

    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int eventId)
    {
        var userId = _userManager.GetUserId(User);
        var eventToRegister = await _eventService.GetByIdAsync(eventId);

        if (eventToRegister == null)
        {
            return NotFound();
        }

        // Проверка дали потребителят вече не е записан
        var isAlreadyRegistered = await _context.EventParticipants
            .AnyAsync(p => p.EventId == eventId && p.UserId == userId);

        if (isAlreadyRegistered)
        {
            TempData["ErrorMessage"] = "Вече сте записани за това събитие.";
            return RedirectToAction("Details", new { id = eventId });
        }

        // Проверка за свободни места
        var participantsCount = await _context.EventParticipants.CountAsync(p => p.EventId == eventId);
        if (participantsCount >= eventToRegister.MaxParticipants)
        {
            TempData["ErrorMessage"] = "Няма свободни места за това събитие.";
            return RedirectToAction("Details", new { id = eventId });
        }

        var participant = new EventParticipant
        {
            EventId = eventId,
            UserId = userId
        };

        _context.EventParticipants.Add(participant);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Вие се записахте успешно за събитието!";
        return RedirectToAction("Details", new { id = eventId });
    }

    [Authorize]
    public async Task<IActionResult> MyEvents()
    {
        var userId = _userManager.GetUserId(User);

        var myEvents = await _context.EventParticipants
            .Where(p => p.UserId == userId)
            .Include(p => p.Event.Category)
            .Select(p => p.Event)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return View(myEvents);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventViewModel model)
    {
        if (ModelState.IsValid)
        {
            var eventToCreate = new Event
            {
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Price = model.Price,
                MaxParticipants = model.MaxParticipants,
                CategoryId = model.CategoryId,
                UserId = _userManager.GetUserId(User)
            };

            await _eventService.CreateAsync(eventToCreate, model.ImageFile);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var eventToEdit = await _eventService.GetByIdAsync(id);
        if (eventToEdit == null) return NotFound();

        if (eventToEdit.UserId != _userManager.GetUserId(User)) return Forbid();

        var model = new EventViewModel
        {
            Id = eventToEdit.Id,
            Name = eventToEdit.Name,
            Description = eventToEdit.Description,
            Date = eventToEdit.Date,
            Location = eventToEdit.Location,
            Price = eventToEdit.Price,
            MaxParticipants = eventToEdit.MaxParticipants,
            CategoryId = eventToEdit.CategoryId,
            ImageUrl = eventToEdit.ImageUrl,
        };

        ViewBag.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventViewModel model)
    {
        if (id != model.Id) return NotFound();

        var eventToUpdate = await _eventService.GetByIdAsync(id);
        if (eventToUpdate == null) return NotFound();

        if (eventToUpdate.UserId != _userManager.GetUserId(User)) return Forbid();

        if (ModelState.IsValid)
        {
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

        ViewBag.Categories = new SelectList(await _eventService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var eventToDelete = await _eventService.GetByIdAsync(id);

        if (eventToDelete == null || eventToDelete.UserId != _userManager.GetUserId(User))
        {
            return NotFound();
        }

        return View(eventToDelete);
    }

    [HttpGet]
    public async Task<IActionResult> Favorites([FromQuery] List<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return View(new List<Event>());
        }

        var favoriteEvents = await _context.Events
            .Where(e => ids.Contains(e.Id))
            .Include(e => e.Category)
            .ToListAsync();

        return View(favoriteEvents);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        await _eventService.DeleteAsync(id, userId);
        return RedirectToAction(nameof(Index));
    }
}