using System.Text.Json;
using InsurancePlatform.Api.Services;

namespace InsurancePlatform.Api.Security;

public sealed class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;

    public AuthTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            await WriteUnauthorizedAsync(context, "Authorization header is required.");
            return;
        }

        var headerValue = authorizationHeader.ToString();
        if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorizedAsync(context, "Bearer token is required.");
            return;
        }

        var token = headerValue["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            await WriteUnauthorizedAsync(context, "Bearer token is required.");
            return;
        }

        var user = await authService.GetUserByTokenAsync(token, context.RequestAborted);
        if (user is null)
        {
            await WriteUnauthorizedAsync(context, "Authentication token is invalid or expired.");
            return;
        }

        context.Items[HttpContextExtensions.CurrentUserItemKey] = user;
        await _next(context);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            new { error = message },
            cancellationToken: context.RequestAborted);
    }
}
