using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Homy.Web.Controllers;
using Homy.Domin.Contract_Service;
using Homy.Application.Dtos;
using Homy.Domin.models;
using Homy.Application.Contract_Service;
using Homy.Application.Dtos.UserDtos;

namespace Homy.UnitTests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUser_Service> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUser_Service>();
            _controller = new UsersController(_mockUserService.Object);
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(), 
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithUsers_WhenServiceReturnsSuccess()
        {
            // Arrange
            var filter = new UserFilterDto();
            var users = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), FullName = "User 1" }
            };
            var response = new PaginatedResponse<UserDto>
            {
                Success = true,
                Data = users,
                TotalCount = 1
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Index(filter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<UserDto>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_ReturnsViewEmpty_WhenServiceReturnsFailure()
        {
            // Arrange
            var filter = new UserFilterDto();
            var response = new PaginatedResponse<UserDto>
            {
                Success = false,
                Message = "Error"
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync(filter))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Index(filter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<UserDto>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Details_ReturnsViewWithUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, FullName = "User 1" };
            var response = new ApiResponse<UserDto>
            {
                Success = true,
                Data = user
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<UserDto>(viewResult.Model);
            Assert.Equal(userId, model.Id);
        }

        [Fact]
        public async Task ToggleActive_ReturnsJsonSuccess_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var isActive = true;
            var response = new ApiResponse<bool>
            {
                Success = true,
                Message = "Updated successfully"
            };

            _mockUserService.Setup(s => s.UpdateUserActiveStatusAsync(It.Is<UpdateUserStatusDto>(
                x => x.UserId == userId && x.IsActive == isActive)))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ToggleActive(userId, isActive);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic data = jsonResult.Value;
            Assert.True((bool)data.GetType().GetProperty("success").GetValue(data, null));
        }
    }
}
