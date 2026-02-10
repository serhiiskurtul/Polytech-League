using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CampusLeague.Application.Services.Implementations;
using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Exceptions;

namespace CampusLeague.Tests.Services
{
    public class RefreshTokenServiceTests
    {
        private readonly RefreshTokenService _service;
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var mockLogger = new Mock<ILogger<RefreshTokenService>>();
            _service = new RefreshTokenService(_dbContext, mockLogger.Object);
        }

        [Fact]
        public async Task GetActiveTokenByUserIdAsync_ShouldReturnNewestActiveToken()
        {
            // Arrange: токени з різним станом
            var tokens = new[]
            {
                new RefreshToken
                {
                    Id = 1L,
                    UserId = 1L,
                    Token = "active1",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsRevoked = false
                },
                new RefreshToken
                {
                    Id = 2L,
                    UserId = 1L,
                    Token = "active2",
                    ExpiresAt = DateTime.UtcNow.AddHours(2),
                    IsRevoked = false
                },
                new RefreshToken
                {
                    Id = 3L,
                    UserId = 1L,
                    Token = "revoked",
                    ExpiresAt = DateTime.UtcNow.AddHours(3),
                    IsRevoked = true
                },
                new RefreshToken
                {
                    Id = 4L,
                    UserId = 2L,
                    Token = "otherUser",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsRevoked = false
                }
            };
            _dbContext.RefreshTokens.AddRange(tokens);
            await _dbContext.SaveChangesAsync();

            // Act
            var resultUser1 = await _service.GetActiveTokenByUserIdAsync(1L);
            var resultUser2 = await _service.GetActiveTokenByUserIdAsync(2L);

            // Assert: повертає найновіший активний токен
            Assert.NotNull(resultUser1);
            Assert.Equal("active2", resultUser1!.Token);

            Assert.NotNull(resultUser2);
            Assert.Equal("otherUser", resultUser2!.Token);
        }

        [Fact]
        public async Task GetActiveTokenByUserIdAsync_ShouldReturnNull_WhenNoActiveToken()
        {
            // Arrange: токен, який вже відкликаний
            var token = new RefreshToken
            {
                Id = 1L,
                UserId = 1L,
                Token = "revoked",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = true
            };
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetActiveTokenByUserIdAsync(1L);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldReturnToken_WhenExists()
        {
            var token = new RefreshToken
            {
                Id = 1L,
                UserId = 1L,
                Token = "mytoken",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = false
            };
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetByRefreshTokenAsync("mytoken");

            Assert.NotNull(result);
            Assert.Equal(token.Token, result.Token);
        }

        [Fact]
        public async Task GetByRefreshTokenAsync_ShouldThrow_WhenNotFound()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.GetByRefreshTokenAsync("notexist")
            );
        }

        [Fact]
        public async Task RevokeAsync_ShouldMarkTokenAsRevoked()
        {
            var token = new RefreshToken
            {
                Id = 1L,
                UserId = 1L,
                Token = "torevoke",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = false
            };
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            await _service.RevokeAsync("torevoke");

            var updatedToken = await _dbContext.RefreshTokens.FindAsync(1L);
            Assert.True(updatedToken!.IsRevoked);
        }

        [Fact]
        public async Task RevokeAsync_ShouldThrow_WhenTokenNotFound()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.RevokeAsync("unknown")
            );
        }

        [Fact]
        public void UpdateValues_ShouldCopyIsRevoked()
        {
            var oldToken = new RefreshToken 
            { 
                Id = 1L,
                UserId = 1L,
                Token = "oldtoken",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsRevoked = false
            };
            var newToken = new RefreshToken 
            { 
                Id = 2L,
                UserId = 2L,
                Token = "newtoken",
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                IsRevoked = true
            };

            // Виклик захищеного методу через Reflection
            var method = typeof(RefreshTokenService)
                .GetMethod("UpdateValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method!.Invoke(_service, new object[] { oldToken, newToken });

            Assert.True(oldToken.IsRevoked);
        }
    }
}
