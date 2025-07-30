using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EventManager.Tests.Services
{
    public class EventServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"EventManagerTestDb_{System.Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();


            _context.Events.AddRange(new List<Event>
            {
                new Event { Id = 1, Name = "Test Event 1", UserId = "owner-id", CategoryId = 1, Category = new Category{Id = 1, Name = "Music"} },
                new Event { Id = 2, Name = "Another Event", UserId = "another-user-id", CategoryId = 2, Category = new Category{Id = 2, Name = "Sport"} }
            });
            _context.SaveChanges();

            _eventService = new EventService(_context, _webHostEnvironmentMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
        {

            var result = await _eventService.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEvent()
        {

            var newEvent = new Event { Name = "New Concert", UserId = "user-id" };

 
            await _eventService.CreateAsync(newEvent, null);


            _context.Events.Should().HaveCount(3);
            _context.Events.FirstOrDefault(e => e.Name == "New Concert").Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEventData()
        {
 
            var eventToUpdate = await _context.Events.FindAsync(1);
            eventToUpdate.Name = "Updated Name";


            await _eventService.UpdateAsync(eventToUpdate, null);
            var updatedEvent = await _context.Events.FindAsync(1);


            updatedEvent.Should().NotBeNull();
            updatedEvent.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_AndRemoveEvent_WhenUserIsOwner()
        {

            var result = await _eventService.DeleteAsync(1, "owner-id", false);


            result.Should().BeTrue();
            (await _context.Events.FindAsync(1)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotOwner()
        {

            var result = await _eventService.DeleteAsync(1, "not-the-owner-id", false);


            result.Should().BeFalse();
            (await _context.Events.FindAsync(1)).Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserIsAdmin()
        {

            var result = await _eventService.DeleteAsync(1, "admin-id", true);


            result.Should().BeTrue();
            (await _context.Events.FindAsync(1)).Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredEvents_WhenSearchTermIsProvided()
        {

            var searchTerm = "Event 1";

            var (resultEvents, totalCount) = await _eventService.GetAllAsync(searchTerm, null, 1, 10);
            totalCount.Should().Be(1);
            resultEvents.Should().HaveCount(1);
            resultEvents.First().Name.Should().Be("Test Event 1");
        }
    }
}