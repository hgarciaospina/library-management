using FluentValidation;
using System.Net;
using System.Text.Json;

namespace LibraryManagement.Api.Middleware
{
    /// <summary>
    /// Global middleware that captures exceptions and converts them into friendly JSON responses.
    /// Handles validation errors (FluentValidation) and unexpected server errors.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue with the pipeline
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // Handle validation errors from FluentValidation
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = new
                {
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage).ToList()
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                // Handle unhandled exceptions
                _logger.LogError(ex, "Unhandled exception occurred");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    Message = "An unexpected error occurred. Please contact support.",
                    Detail = ex.Message // ⚠️ Consider removing in production
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    /// <summary>
    /// Extension method to easily register middleware in Program.cs
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
