using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CampusLeague.Application.Services.Implementations;
using CampusLeague.Application.Security;
using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Exceptions;
using System.Security.Authentication;
using CampusLeague.Persistence.Database;

namespace CampusLeague.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _service;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<IPasswordEncoder> _mockPasswordEncoder;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var mockLogger = new Mock<ILogger<UserService>>();
            _mockPasswordEncoder = new Mock<IPasswordEncoder>();

            _service = new UserService(_dbContext, _mockPasswordEncoder.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetAllWithDeletedAsync_ShouldReturnAllUsers()
        {
            // Arrange
            _dbContext.Users.AddRange(
                new User { 
                    Id = 1, 
                    Email = "a@example.com",
                    IsDeleted = false,
                    FirstName = "First",
                    LastName = "Last",
                    Password = "password"
                },
                new User { 
                    Id = 2, 
                    Email = "b@example.com", 
                    IsDeleted = true,
                    FirstName = "First",
                    LastName = "Last",
                    Password = "password"
                }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllWithDeletedAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { 
                Email = "test@example.com", 
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Password = "password"   
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetByEmailAsync("test@example.com");

            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldThrow_WhenUserNotFound()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.GetByEmailAsync("notfound@example.com")
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldEncodePassword_AndAddUser()
        {
            _mockPasswordEncoder.Setup(pe => pe.Encode("raw")).Returns("encoded");

            var user = new User { 
                Email = "new@example.com", 
                Password = "raw",
                FirstName = "First",
                LastName = "Last",
            };

            var result = await _service.CreateAsync(user);

            Assert.Equal("encoded", result.Password);
            Assert.Single(_dbContext.Users);
            Assert.Equal("new@example.com", _dbContext.Users.First().Email);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            _dbContext.Users.Add(new User { 
                Email = "exists@example.com",
                Password = "encoded",
                FirstName = "First",
                LastName = "Last",
                Role = UserRole.User
            });
            await _dbContext.SaveChangesAsync();

            var user = new User { 
                Email = "exists@example.com", 
                FirstName = "First", 
                LastName = "Last", 
                Role = UserRole.User, 
                Password = "raw" 
            };

            await Assert.ThrowsAsync<AuthenticationException>(
                () => _service.CreateAsync(user)
            );
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldMarkUserAsDeleted()
        {
            var user = new User { 
                Id = 1L,
                Email = "softdelete@example.com",
                IsDeleted = false,
                Password = "ShouldNotChange",
                Role = UserRole.User,
                FirstName = "First",
                LastName = "Last"
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            await _service.SoftDeleteAsync(1L);

            var updatedUser = await _dbContext.Users.FindAsync(1L);
            Assert.True(updatedUser!.IsDeleted);
        }


        [Fact]
        public async Task SoftDeleteAsync_ShouldThrow_WhenUserNotFound()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.SoftDeleteAsync(999)
            );
        }

        [Fact]
        public void UpdateValues_ShouldCopyFieldsCorrectly()
        {
            var oldUser = new User
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                Role = UserRole.User,
                Password = "ShouldNotChange"
            };

            var newUser = new User
            {
                FirstName = "New",
                LastName = "Surname",
                Email = "new@example.com",
                Role = UserRole.Admin,
                Password = "ShouldNotChange"
            };

            // Виклик захищеного методу через наслідування
            var method = typeof(UserService)
                .GetMethod("UpdateValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method!.Invoke(_service, new object[] { oldUser, newUser });

            Assert.Equal("New", oldUser.FirstName);
            Assert.Equal("Surname", oldUser.LastName);
            Assert.Equal("new@example.com", oldUser.Email);
            Assert.Equal(UserRole.Admin, oldUser.Role);
        }
    }
}
