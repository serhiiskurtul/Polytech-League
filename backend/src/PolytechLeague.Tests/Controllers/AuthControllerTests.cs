using System.Net;
using System.Net.Http.Json;
using PolytechLeague.Application.Dto;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Tests.TestInfrastructure.Authentication;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Authentication;

namespace PolytechLeague.Tests.IntegrationTests;

public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(PolytechLeagueWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndSetsCookies()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        var tokens = new AuthResponse { AccessToken = "access-token-123", RefreshToken = "refresh-token-123" };
        authServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<AuthRequest>()))
            .ReturnsAsync(tokens);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);
            });
        }).CreateClient();

        var request = new AuthRequest { Email = "test@example.com", Password = "password" };

        // Act
        var response = await client.PostAsJsonAsync("/api/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();
        responseBody.Should().NotBeNull();
        responseBody!.AccessToken.Should().Be("access-token-123");
        responseBody!.RefreshToken.Should().Be("refresh-token-123");

        // Check cookies
        response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders);
        cookieHeaders.Should().NotBeNull();
        var cookies = cookieHeaders!.ToList();
        cookies.Any(c => c.Contains("access_token=access-token-123")).Should().BeTrue();
        cookies.Any(c => c.Contains("refresh_token=refresh-token-123")).Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<AuthRequest>()))
            .ThrowsAsync(new AuthenticationException("Invalid email or password"));

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);
            });
        }).CreateClient();

        var request = new AuthRequest { Email = "invalid@example.com", Password = "wrong" };

        // Act
        var response = await client.PostAsJsonAsync("/api/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithTokenInBody_ReturnsOkAndSetsCookies()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        var tokens = new AuthResponse { AccessToken = "new-access-token", RefreshToken = "new-refresh-token" };
        authServiceMock.Setup(s => s.RevokeAndRefreshJwtTokensAsync(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokens);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);
            });
        }).CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "old-refresh-token" };

        // Act
        var response = await client.PostAsJsonAsync("/api/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();
        responseBody.Should().NotBeNull();
        responseBody!.AccessToken.Should().Be("new-access-token");
        responseBody!.RefreshToken.Should().Be("new-refresh-token");

        response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders);
        cookieHeaders.Should().NotBeNull();
        var cookies = cookieHeaders!.ToList();
        cookies.Any(c => c.Contains("access_token=new-access-token")).Should().BeTrue();
        cookies.Any(c => c.Contains("refresh_token=new-refresh-token")).Should().BeTrue();
    }

    [Fact]
    public async Task Refresh_WithTokenInCookie_ReturnsOkAndSetsCookies()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        var tokens = new AuthResponse { AccessToken = "new-access-token", RefreshToken = "new-refresh-token" };
        authServiceMock.Setup(s => s.RevokeAndRefreshJwtTokensAsync(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokens);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);
            });
        }).CreateClient();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/refresh");
        requestMessage.Headers.Add("Cookie", "refresh_token=old-refresh-token");
        requestMessage.Content = JsonContent.Create((RefreshTokenRequest?)null);

        // Act
        var response = await client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadFromJsonAsync<AuthResponse>();
        responseBody.Should().NotBeNull();
        responseBody!.AccessToken.Should().Be("new-access-token");
        responseBody!.RefreshToken.Should().Be("new-refresh-token");

        response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders);
        cookieHeaders.Should().NotBeNull();
        var cookies = cookieHeaders!.ToList();
        cookies.Any(c => c.Contains("access_token=new-access-token")).Should().BeTrue();
        cookies.Any(c => c.Contains("refresh_token=new-refresh-token")).Should().BeTrue();
    }

    [Fact]
    public async Task Refresh_WithMissingToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/refresh");
        requestMessage.Content = JsonContent.Create((RefreshTokenRequest?)null);

        // Act
        var response = await client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_DeletesCookiesAndReturnsUnauthorized()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.RevokeAndRefreshJwtTokensAsync(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Security.Authentication.AuthenticationException("Invalid token"));

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);
            });
        }).CreateClient();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/refresh");
        requestMessage.Headers.Add("Cookie", "refresh_token=invalid-token");
        requestMessage.Content = JsonContent.Create((RefreshTokenRequest?)null);

        // Act
        var response = await client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders);
        cookieHeaders.Should().NotBeNull();
        var cookies = cookieHeaders!.ToList();
        cookies.Any(c => c.Contains("access_token=;")).Should().BeTrue();
        cookies.Any(c => c.Contains("refresh_token=;")).Should().BeTrue();
    }

    [Fact]
    public async Task Logout_WithAuth_RevokesTokenAndDeletesCookies()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IAuthService));
                services.AddSingleton(authServiceMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<MockAuthenticationHandlerOptions, TestAuthHandler>("TestScheme", options => { });

                services.Configure<MockAuthenticationHandlerOptions>(options => options.AuthRole = UserRole.User);
            });
        }).CreateClient();

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/logout");
        requestMessage.Headers.Add("Cookie", "refresh_token=old-refresh-token");

        // Act
        var response = await client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        authServiceMock.Verify(s => s.RevokeRefreshTokenAsync(It.Is<RefreshTokenRequest>(r => r.RefreshToken == "old-refresh-token"), It.IsAny<CancellationToken>()), Times.Once);

        response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders);
        cookieHeaders.Should().NotBeNull();
        var cookies = cookieHeaders!.ToList();
        cookies.Any(c => c.Contains("access_token=;")).Should().BeTrue();
        cookies.Any(c => c.Contains("refresh_token=;")).Should().BeTrue();
    }

    [Fact]
    public async Task Logout_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/logout");

        // Act
        var response = await client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
