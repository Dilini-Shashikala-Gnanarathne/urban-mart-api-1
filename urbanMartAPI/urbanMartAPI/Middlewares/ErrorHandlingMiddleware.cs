
using System.Text.Json;
using urbanMartAPI.Models;

namespace urbanMartAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleErrorAsync(context, StatusCodes.Status401Unauthorized, "You are not authorized.", ex);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(context, StatusCodes.Status500InternalServerError, "An error occurred while processing your request.", ex);
            }
        }

        private static async Task HandleErrorAsync(HttpContext context, int statusCode, string message, Exception? ex = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            ApiResponse<object> response = ApiResponse<object>.ErrorResponse(message, Array.Empty<object>());
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}