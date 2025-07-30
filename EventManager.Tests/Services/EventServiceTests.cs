using EventManagementSystem.Data;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using EventManagementSystem.Services.Contracts; 
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        private readonly Mock<IFileStorageService> _fileStorageServiceMock; 
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"EventManagerTestDb_{System.Guid.NewGuid()}")
                .Options;
            _context = new ApplicationDbContext(options);

            _fileStorageServiceMock = new Mock<IFileStorageService>();


            _context.Events.AddRange(new List<Event>
            {
                new Event { Id = 1, Name = "Test Event 1", UserId = "owner-id", CategoryId = 1, Category = new Category{Id = 1, Name = "Music"} },
                new Event { Id = 2, Name = "Another Event", UserId = "another-user-id", CategoryId = 2, Category = new Category{Id = 2, Name = "Sport"} }
            });
            _context.SaveChanges();


            _eventService = new EventService(_context, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallUpload_WhenImageFileIsProvided()
        {

            var newEvent = new Event { Name = "New Concert", UserId = "user-id" };
            var mockFile = new Mock<IFormFile>();


            mockFile.Setup(f => f.Length).Returns(1);

            _fileStorageServiceMock
                .Setup(f => f.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync("http://dropbox.com/image.jpg");


            await _eventService.CreateAsync(newEvent, mockFile.Object);


            _fileStorageServiceMock.Verify(f => f.UploadAsync(mockFile.Object, It.IsAny<string>()), Times.Once);
            _context.Events.FirstOrDefault(e => e.Name == "New Concert").ImageUrl.Should().Be("http://dropbox.com/image.jpg");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
        {

            var result = await _eventService.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_AndRemoveEvent_WhenUserIsOwner()
        {

            var result = await _eventService.DeleteAsync(1, "owner-id", false);

            result.Should().BeTrue();
            (await _context.Events.FindAsync(1)).Should().BeNull();
        }
    }
}