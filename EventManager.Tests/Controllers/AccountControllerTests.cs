using EventManagementSystem.Controllers;
using EventManagementSystem.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EventManager.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async Task Register_Post_ShouldRedirectToHome_WhenRegistrationIsSuccessful()
        {

            var model = new RegisterModel { Username = "testuser", Email = "test@test.com", Password = "Password123" };
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(model);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_Post_ShouldReturnViewWithModelError_WhenLoginFails()
        {
            var model = new LoginModel { Username = "testuser", Password = "wrongpassword" };
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(model.Username, model.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(model);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            _controller.ModelState.ErrorCount.Should().Be(1);
            _controller.ModelState[""].Errors[0].ErrorMessage.Should().Be("Невалиден опит за вход.");
        }
    }
}