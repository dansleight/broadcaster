namespace Broadcaster;

public class NormalizeAuthorizationHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public NormalizeAuthorizationHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            // If the header is a valid token without "Bearer ", prepend it
            if (!authHeader.Contains(" ")) // Simple check to avoid misinterpreting other schemes
            {
                context.Request.Headers["Authorization"] = "Bearer " + authHeader;
            }
        }

        await _next(context);
    }
}
