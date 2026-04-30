using Microsoft.AspNetCore.Http.Features;
using System.Threading.RateLimiting;
using System.Text.Json.Serialization;
using PolytechLeague.Application.JWT;
using PolytechLeague.Infrastructure.Persistence.Database;
using PolytechLeague.Infrastructure.Email;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using StackExchange.Redis;
using PolytechLeague.API.Extensions;
using Serilog;
using PolytechLeague.API.Middlewares;
using Microsoft.OpenApi.Models;
using PolytechLeague.API.Hubs;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", 
                                "http://localhost:3000", 
                                "http://localhost:5174",
                                "https://polytech-league.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});



builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2147483648; // 2GB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2147483648; // 2GB
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services
    .AddPersistence(builder.Configuration)
    .AddServices(builder.Configuration)
    .AddAuth(builder.Configuration);


var redisConnectionString = builder.Configuration["RedisSettings:Redis"];
var options = ConfigurationOptions.Parse(redisConnectionString);
options.Ssl = true;
options.AbortOnConnectFail = false;
var multiplexer = ConnectionMultiplexer.Connect(options);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));


builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddSignalR();



builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromSeconds(10);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;  // якщо 0 — 503, якщо >0 — 429
    });
    options.RejectionStatusCode = 429; // Глобальний код відмови
});

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

//Автоматичне застосування міграцій при старті (корисно для Railway)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PolytechLeague.Persistence.Database.ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка під час застосування міграцій бази даних.");
    }
}

app.Run();


public partial class Program { }
