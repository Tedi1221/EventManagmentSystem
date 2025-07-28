using EventManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Services.Contracts
{
    public interface IEventService
    {
        Task<(IEnumerable<Event> Events, int TotalCount)> GetAllAsync(string? searchTerm, int? categoryId, int page, int pageSize);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event eventToCreate, IFormFile? imageFile);
        Task<Event> UpdateAsync(Event eventToUpdate, IFormFile? imageFile);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        void DeleteImage(string imageUrl);
        Task<string> SaveImageAsync(IFormFile? imageFile);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin = false);
    }
}