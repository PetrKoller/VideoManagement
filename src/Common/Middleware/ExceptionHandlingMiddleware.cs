namespace Common.Middleware;

/// <summary>
/// Global exception handling middleware.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var failure = new Failure("System.InternalServerError", "Internal Server Error.");
            await context.Response.WriteAsJsonAsync(failure);
            logger.LogError(exception, "Unexpected error");
        }
    }
}
