using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Homy.Infurastructure.Repository;
using Homy.Infurastructure.Data;
using Homy.Domin.models;

namespace Homy.UnitTests.Repositories
{
    public class UserRepoTests
    {
        private readonly DbContextOptions<HomyContext> _options;

        public UserRepoTests()
        {
            _options = new DbContextOptionsBuilder<HomyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers_WhenNoFilter()
        {
            // Arrange
            using (var context = new HomyContext(_options))
            {
                context.Users.Add(new User { 
                    Id = Guid.NewGuid(), 
                    FullName = "User 1", 
                    UserName = "user1", 
                    PhoneNumber = "1234567890", 
                    IsVerified = true, 
                    IsActive = true, 
                    Role = UserRole.Agent 
                });
                context.Users.Add(new User { 
                    Id = Guid.NewGuid(), 
                    FullName = "User 2", 
                    UserName = "user2", 
                    PhoneNumber = "0987654321", 
                    IsVerified = false, 
                    IsActive = true, 
                    Role = UserRole.Owner 
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HomyContext(_options))
            {
                var repo = new UserRepo(context);
                var result = await repo.GetAllUsersAsync();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetAllUsersAsync_FiltersByRole()
        {
            // Arrange
            using (var context = new HomyContext(_options))
            {
                context.Users.Add(new User { Id = Guid.NewGuid(), UserName = "u3", PhoneNumber = "111", Role = UserRole.Agent, FullName = "U3" });
                context.Users.Add(new User { Id = Guid.NewGuid(), UserName = "u4", PhoneNumber = "222", Role = UserRole.Owner, FullName = "U4" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HomyContext(_options))
            {
                var repo = new UserRepo(context);
                var result = await repo.GetAllUsersAsync(role: UserRole.Agent);

                // Assert
                Assert.Single(result);
                Assert.Equal(UserRole.Agent, result.First().Role);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenExists()
        {
             // Arrange
            var userId = Guid.NewGuid();
            using (var context = new HomyContext(_options))
            {
                context.Users.Add(new User { Id = userId, FullName = "Target User", UserName = "target", PhoneNumber = "333" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HomyContext(_options))
            {
                var repo = new UserRepo(context);
                var result = await repo.GetUserByIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.Id);
            }
        }

        [Fact]
        public async Task UpdateActiveStatusAsync_UpdatesStatus()
        {
             // Arrange
            var userId = Guid.NewGuid();
            using (var context = new HomyContext(_options))
            {
                context.Users.Add(new User { Id = userId, IsActive = false, UserName = "update_user", PhoneNumber = "444", FullName = "Update User" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HomyContext(_options))
            {
                var repo = new UserRepo(context);
                var result = await repo.UpdateActiveStatusAsync(userId, true);
                
                // Assert
                Assert.True(result);
                var user = await context.Users.FindAsync(userId);
                Assert.NotNull(user);
                Assert.True(user.IsActive);
            }
        }
    }
}
