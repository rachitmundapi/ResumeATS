using System.Net;
using System.Text.Json;

namespace ResumeATS.Middleware;

/// <summary>
/// Catches all unhandled exceptions in the pipeline and returns a consistent
/// RFC 7807 Problem Details JSON response instead of exposing stack traces.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by global middleware.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            HttpRequestException httpEx when httpEx.StatusCode.HasValue
                => ((int)httpEx.StatusCode.Value, "Upstream API Error"),
            InvalidOperationException
                => (StatusCodes.Status502BadGateway, "Processing Error"),
            OperationCanceledException
                => (StatusCodes.Status408RequestTimeout, "Request Cancelled"),
            _   => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = statusCode;

        var problem = new
        {
            type   = $"https://httpstatuses.io/{statusCode}",
            title,
            status = statusCode,
            detail = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
