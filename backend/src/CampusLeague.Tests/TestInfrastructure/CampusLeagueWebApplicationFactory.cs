using DotNet.Testcontainers.Builders;
using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Params;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.PostgreSql;
using Xunit;
using CampusLeague.Infrastructure.Persistence.Database;
using CampusLeague.Infrastructure.Redis;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Application.Dto;
using CampusLeague.Tests.TestInfrastructure.Authentication;

namespace CampusLeague.Tests.IntegrationTests;

public class CampusLeagueWebApplicationFactory :
    WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly int _portNumber = 5432;
    private readonly PostgreSqlContainer _dbContainer;

    public UserRole AuthRole { get; private set; } = UserRole.Admin;

    public CampusLeagueWebApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16.4")
            .WithDatabase("CampusLeague_db")
            .WithPortBinding(_portNumber, assignRandomHostPort: true)
            .WithUsername("root")
            .WithPassword("1234")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(_portNumber))
            .Build();
    }

    public CampusLeagueWebApplicationFactory WithRole(UserRole role)
    {
        AuthRole = role;
        return this;
    }


    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Після запуску контейнера створюємо схему БД
        using var scope = Services.CreateScope();
        //Отримуємо екземпляр ApplicationDbContext зі створеного scope.
        ////GetRequiredService<T>() означає: якщо такого сервісу немає — буде кинута помилка
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            // Видаляємо реєстрації сервісів, якщо є
            services.RemoveAll(typeof(IRedisService));
            services.RemoveAll(typeof(IUserService));

            // Додаємо мок-реалізації
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetPageAsync(
                    It.IsAny<FilterParams>(),
                    It.IsAny<SortParams>(),
                    It.IsAny<PageParams>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(new PageResult<User>(
                    data: new List<User>(),
                    totalCount: 0
                ));

            var redisServiceMock = new Mock<IRedisService>();
            redisServiceMock.Setup(r => r.GetDataAsync<PageResult<UserResponse>>(It.IsAny<string>()))
                .ReturnsAsync((PageResult<UserResponse>?)null);
            redisServiceMock.Setup(r => r.SetDataAsync(
                    It.IsAny<string>(),
                    It.IsAny<PageResult<UserResponse>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            services.AddSingleton(userServiceMock.Object);
            services.AddSingleton(redisServiceMock.Object);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            services.Configure<MockAuthenticationHandlerOptions>(options => options.AuthRole = AuthRole);
            services.Configure<MockAuthenticationHandlerOptions>(
                options => options.AuthRole = AuthRole);

        });
    }

}
