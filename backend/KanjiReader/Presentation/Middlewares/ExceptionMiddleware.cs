using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KanjiReader.Presentation.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            
            var statusCode = ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
            
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var error = new { Error = ex.Message };
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
