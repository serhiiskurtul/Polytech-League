using System.Security.Authentication;
using System.Text.Json;
using CampusLeague.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CampusLeague.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Could not process a request on machine {MachineName}.",
                Environment.MachineName
            );

            await HandleExceptionAsync(httpContext,ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context, Exception exception)
    {
        ProblemDetails problemDetails;

        problemDetails = exception switch
        {
            EntityNotFoundException => new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = exception.Message,
                Instance = context.Request.Path
            },

            ArgumentException or
            ArgumentNullException or
            ArgumentOutOfRangeException or
            InvalidOperationException => new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message,
                Instance = context.Request.Path
            },

            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = exception.Message,
                Instance = context.Request.Path
            },

            AuthenticationException => new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = exception.Message,
                Instance = context.Request.Path
            },

            OperationCanceledException or TaskCanceledException => new ProblemDetails
            {
                Title = "Client Closed Request",
                Status = StatusCodes.Status499ClientClosedRequest,
                Detail = exception.Message,
                Instance = context.Request.Path
            },

            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message,
                Instance = context.Request.Path
            }  
        };

        HttpResponse response = context.Response;

        response.ContentType = "application/problem+json";
        response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
