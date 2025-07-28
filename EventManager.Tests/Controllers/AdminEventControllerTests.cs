using EventManagementSystem.Areas.Admin.Controllers;
using EventManagementSystem.Models;
using EventManagementSystem.Services.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace EventManager.Tests.Controllers
{
    public class AdminEventControllerTests
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly AdminEventController _controller;

        public AdminEventControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _controller = new AdminEventController(_eventServiceMock.Object, _userManagerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-id"),
            }));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        [Fact]
        public async Task Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
        {

            var model = new EventFormViewModel
            {
                Name = "Test Event",
                Description = "This is a test description.",
                Date = System.DateTime.Now.AddDays(10),
                Location = "Test Location",
                CategoryId = 1,
                Price = 20,
                MaxParticipants = 100
            };

            var result = await _controller.Create(model);

            _eventServiceMock.Verify(s => s.CreateAsync(It.IsAny<Event>(), model.ImageFile), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Create_Post_ShouldReturnViewWithModel_WhenModelStateIsInvalid()
        {
            var model = new EventFormViewModel();
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(model);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(model);
        }

        [Fact]
        public async Task Edit_Post_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            var model = new EventFormViewModel { Id = 1, Name = "Updated Name" };
            _eventServiceMock.Setup(s => s.GetByIdAsync(model.Id)).ReturnsAsync((Event)null);

            var result = await _controller.Edit(model.Id, model);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldCallDeleteService_AndRedirect()
        {
            var eventId = 1;
            _eventServiceMock.Setup(s => s.DeleteAsync(eventId, "admin-id", true)).ReturnsAsync(true);

            var result = await _controller.DeleteConfirmed(eventId);

            _eventServiceMock.Verify(s => s.DeleteAsync(eventId, "admin-id", true), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }
    }
}