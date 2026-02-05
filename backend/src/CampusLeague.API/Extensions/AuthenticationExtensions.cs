using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CampusLeague.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var secretKey = configuration["JwtOptions:SecretKey"] 
            ?? throw new InvalidOperationException("JWT Secret Key is missing in configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["JwtOptions:Issuer"],
                ValidAudience = configuration["JwtOptions:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero, // Видаляємо дефолтні 5 хв затримки
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // 1. Спочатку пробуємо дістати токен з куки
                    var accessToken = context.Request.Cookies["access_token"];

                    // 2. Якщо куки немає, JwtBearer автоматично спробує знайти Header "Authorization: Bearer ..."
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    // Допомагає відлагоджувати помилки (наприклад, ExpiredToken)
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", p => 
                p.RequireRole("Admin")); 

            options.AddPolicy("User", p => 
                p.RequireRole("User"));

            options.AddPolicy("AdminOrUser", p => 
                p.RequireRole("Admin", "User"));
        });

        return services;
    }
}