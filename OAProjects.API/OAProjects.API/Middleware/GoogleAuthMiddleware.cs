namespace OAProjects.API.Middleware;

public class GoogleAuthMiddleware
{
    private readonly RequestDelegate _next;
    public GoogleAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        string token = httpContext.Request.Headers.FirstOrDefault(m => m.Key == "Authorization").Value.ToString() ?? "";

        if (!string.IsNullOrEmpty(token) && false)
        {
        }
        else
        {
            httpContext.Response.Redirect("~/Auth/Unauthorized");
        }

        // Move forward into the pipeline
        await _next(httpContext);
    }
}

public static class GoogleAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseGoogleAuthMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GoogleAuthMiddleware>();
    }
}
