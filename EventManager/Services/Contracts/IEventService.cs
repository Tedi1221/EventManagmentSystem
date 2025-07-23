using EventManagementSystem.Models;

namespace EventManagementSystem.Services.Contracts
{
    public interface IEventService
    {
        Task<(IEnumerable<Event> Events, int TotalCount)> GetAllAsync(string? searchTerm, int? categoryId, int page, int pageSize);
        Task<Event?> GetByIdAsync(int id);
        Task CreateAsync(EventFormViewModel model);
        Task UpdateAsync(EventFormViewModel model);
        Task DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}