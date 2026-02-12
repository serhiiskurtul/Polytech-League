using System.Net;
using System.Net.Http.Json;
using PolytechLeague.Application.Dto;
using PolytechLeague.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PolytechLeague.Tests.IntegrationTests;

public class UsersControllerTests : IntegrationTestBase
{
    public UsersControllerTests(PolytechLeagueWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        Factory.WithRole(UserRole.None);
        RecreateClient();

        // Act
        var response = await httpClient.GetAsync("/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        Factory.WithRole(UserRole.None);
        RecreateClient();

        // Act
        var response = await httpClient.GetAsync("/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllWithDeleted_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        Factory.WithRole(UserRole.None);
        RecreateClient();

        // Act
        var response = await httpClient.GetAsync("/users/with-deleted");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_User_ReturnsCreated()
    {
        // Arrange (AllowAnonymous → роль не важлива, але ставимо явно)
        Factory.WithRole(UserRole.None);
        RecreateClient();

        var request = new UserRequest
        {
            Email = "testuser@gmail.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_User_WithoutEmail_ReturnsCreated()
    {
        // Arrange
        Factory.WithRole(UserRole.None);
        RecreateClient();

        var request = new UserRequest
        {
            Email = null,
            FirstName = "NoEmail",
            LastName = "User",
            Password = "Password123!"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task HardDelete_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        Factory.WithRole(UserRole.None);
        RecreateClient();

        // Act
        var response = await httpClient.DeleteAsync("/users/hard-delete/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
