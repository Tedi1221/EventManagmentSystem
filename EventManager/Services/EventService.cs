using EventManagementSystem.Data;
using EventManagementSystem.Models; // <-- ТАЗИ ДИРЕКТИВА Е КЛЮЧОВА
using EventManagementSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

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

        // ... целият останал код на услугата остава същият ...
        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetAllAsync(string? searchTerm, int? categoryId, int page, int pageSize)
        {
            var query = _context.Events
                .Include(e => e.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm) || e.Description.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();

            var events = await query
                .OrderByDescending(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (events, totalCount);
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(EventFormViewModel model)
        {
            string imageUrl = await SaveImageAsync(model.ImageFile);

            var newEvent = new Event
            {
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Price = model.Price,
                MaxParticipants = model.MaxParticipants,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl
            };

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventFormViewModel model)
        {
            var eventToUpdate = await _context.Events.FindAsync(model.Id);
            if (eventToUpdate == null) return;

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(eventToUpdate.ImageUrl) && !eventToUpdate.ImageUrl.Contains("default-event.jpg"))
                {
                    DeleteImage(eventToUpdate.ImageUrl);
                }
                eventToUpdate.ImageUrl = await SaveImageAsync(model.ImageFile);
            }

            eventToUpdate.Name = model.Name;
            eventToUpdate.Description = model.Description;
            eventToUpdate.Date = model.Date;
            eventToUpdate.Location = model.Location;
            eventToUpdate.Price = model.Price;
            eventToUpdate.MaxParticipants = model.MaxParticipants;
            eventToUpdate.CategoryId = model.CategoryId;

            _context.Events.Update(eventToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete != null)
            {
                if (!string.IsNullOrEmpty(eventToDelete.ImageUrl) && !eventToDelete.ImageUrl.Contains("default-event.jpg"))
                {
                    DeleteImage(eventToDelete.ImageUrl);
                }
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return "/images/default-event.jpg";
            }

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

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}