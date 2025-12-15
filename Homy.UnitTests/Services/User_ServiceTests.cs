using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Homy.Infurastructure.Service;
using Homy.Application.Dtos;
using Homy.Domin.models;
using Homy.Infurastructure.Unitofworks;
using Homy.Domin.Contract_Repo;
using Homy.Application.Service;
using Homy.Application.Dtos.UserDtos;

namespace Homy.UnitTests.Services
{
    public class User_ServiceTests
    {
        private readonly Mock<IUnitofwork> _mockUnitOfWork;
        private readonly Mock<IUserRepo> _mockUserRepo;
        private readonly User_Service _userService;

        public User_ServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitofwork>();
            _mockUserRepo = new Mock<IUserRepo>();

            _mockUnitOfWork.Setup(u => u.UserRepo).Returns(_mockUserRepo.Object);

            _userService = new User_Service(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsPaginatedUsers_WhenUsersExist()
        {
            // Arrange
            var filter = new UserFilterDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FullName = "User 1", Role = UserRole.Agent },
                new User { Id = Guid.NewGuid(), FullName = "User 2", Role = UserRole.Owner }
            };

            _mockUserRepo.Setup(repo => repo.GetAllUsersAsync(
                It.IsAny<UserRole?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<string>()
            )).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync(filter);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FullName = "Test User" };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsError_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("المستخدم غير موجود", result.Message);
        }

        [Fact]
        public async Task VerifyAgentAsync_ReturnsSuccess_WhenAgentExistsAndIsVerified()
        {
             // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Role = UserRole.Agent };
            var request = new VerificationRequestDto { UserId = userId, IsApproved = true };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            
            _mockUserRepo.Setup(repo => repo.UpdateVerificationStatusAsync(userId, true, null))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.VerifyAgentAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task VerifyAgentAsync_ReturnsError_WhenUserIsNotAgent()
        {
             // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Role = UserRole.Owner };
            var request = new VerificationRequestDto { UserId = userId, IsApproved = true };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            
            // Act
            var result = await _userService.VerifyAgentAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("المستخدم ليس سمسار", result.Message);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsError_WhenUserIsAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Role = UserRole.Admin };

            _mockUserRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("لا يمكن حذف حساب الأدمن", result.Message);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
        }
    }
}
