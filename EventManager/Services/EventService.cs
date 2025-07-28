using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementSystem.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EventService(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetAllAsync(string? searchTerm, int? categoryId, int page, int pageSize)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.Name.Contains(searchTerm) ||
                    e.Description.Contains(searchTerm) ||
                    e.Location.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            var events = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (events, totalCount);
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateAsync(Event eventToCreate, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                eventToCreate.ImageUrl = await SaveImageAsync(imageFile);
            }
            else
            {
                eventToCreate.ImageUrl = "/images/default-event.jpg";
            }
            _context.Events.Add(eventToCreate);
            await _context.SaveChangesAsync();
            return eventToCreate;
        }

        public async Task<Event> UpdateAsync(Event eventToUpdate, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(eventToUpdate.ImageUrl) && !eventToUpdate.ImageUrl.Equals("/images/default-event.jpg"))
                {
                    DeleteImage(eventToUpdate.ImageUrl);
                }
                eventToUpdate.ImageUrl = await SaveImageAsync(imageFile);
            }
            _context.Events.Update(eventToUpdate);
            await _context.SaveChangesAsync();
            return eventToUpdate;
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin = false)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null) return false;

            if (!isAdmin && eventToDelete.UserId != userId)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(eventToDelete.ImageUrl))
            {
                DeleteImage(eventToDelete.ImageUrl);
            }
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || imageUrl.Equals("/images/default-event.jpg")) return;
            var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return "/images/default-event.jpg";
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "events");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return $"/images/events/{uniqueFileName}";
        }
    }
}