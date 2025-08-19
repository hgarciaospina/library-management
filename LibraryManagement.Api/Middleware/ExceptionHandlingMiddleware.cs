using FluentValidation;
using System.Net;
using System.Text.Json;

namespace LibraryManagement.Api.Middleware
{
    /// <summary>
    /// Global middleware that captures exceptions and converts them into friendly JSON responses.
    /// It handles both validation errors (from FluentValidation) and unexpected server errors.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger to log errors and warnings.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware by executing the next middleware in the pipeline, 
        /// or catching and handling exceptions if they occur.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue with the next middleware in the pipeline.
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // Handle validation errors from FluentValidation.
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = new
                {
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage).ToList()  // Return validation error messages
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                // Handle unexpected server errors.
                _logger.LogError(ex, "Unhandled exception occurred");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    Message = "An unexpected error occurred. Please contact support.",
                    Detail = ex.Message  // ⚠️ Be cautious with exposing exception details in production.
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    /// <summary>
    /// Extension method to register the exception handling middleware easily in Program.cs.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Registers the <see cref="ExceptionHandlingMiddleware"/> in the application's middleware pipeline.
        /// </summary>
        /// <param name="builder">The application's builder.</param>
        /// <returns>The updated application builder.</returns>
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
