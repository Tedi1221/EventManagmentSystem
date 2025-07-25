// Път: Services/Contracts/IEventService.cs
using EventManagementSystem.Models; // <-- ТАЗИ ДИРЕКТИВА Е КЛЮЧОВА

namespace EventManagementSystem.Services.Contracts
{
    public interface IEventService
    {
        Task<(IEnumerable<Event> Events, int TotalCount)> GetAllAsync(string? searchTerm, int? categoryId, int page, int pageSize);
        Task<Event?> GetByIdAsync(int id);
        Task CreateAsync(EventFormViewModel model); // <-- Използва ViewModel
        Task UpdateAsync(EventFormViewModel model); // <-- Използва ViewModel
        Task DeleteAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
