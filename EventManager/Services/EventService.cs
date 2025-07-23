using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

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
            var newEvent = new Event
            {
                Name = model.Name,
                Description = model.Description,
                Date = model.Date,
                Location = model.Location,
                Price = model.Price,
                MaxParticipants = model.MaxParticipants,
                CategoryId = model.CategoryId
            };

            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventFormViewModel model)
        {
            var eventToUpdate = await _context.Events.FindAsync(model.Id);
            if (eventToUpdate != null)
            {
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
        }

        public async Task DeleteAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
