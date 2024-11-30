using book_rental_api.Exceptions;
using book_rental_api.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;

namespace book_rental_api.Middlewares
{
    public class APIMiddleware : IMiddleware
    {
        private readonly ILogger<APIMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public APIMiddleware(ILogger<APIMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate _next)
        {
            try
            {
                await _next(context);

                LogSuccess(context);
            }

            catch (ModelValidationException ex)
            {
                LogError(context, ex.StatusCode, ex.Message, ex);

                await WriteResponseAsync(
                    context,
                    ex.StatusCode,
                    ex.Message,
                    _env.IsDevelopment() ? ex.StackTrace : null,
                    ex.Errors
                    );
            }
            catch (APIException ex)
            {
                LogError(context, ex.StatusCode, ex.Message, ex);

                await WriteResponseAsync(
                    context,
                    ex.StatusCode,
                    ex.Message,
                    _env.IsDevelopment() ? ex.StackTrace : null,
                    null
                    );
            }
            catch (Exception ex)
            {
                const int internalServerError = (int)HttpStatusCode.InternalServerError;

                string message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred";

                LogError(context, internalServerError, ex.Message, ex);

                await WriteResponseAsync(
                    context,
                    internalServerError,
                    message,
                    _env.IsDevelopment() ? ex.StackTrace : null,
                    null
                    );
            }
        }


        private async Task WriteResponseAsync(HttpContext context, int statusCode, string message, string? stackTrace, IDictionary<string, string[]>? errors)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            ErrorResponse response = new()
            {
                StatusCode = statusCode,
                Message = message,
                StackTrace = stackTrace,
                Errors = errors
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private void LogSuccess(HttpContext context)
        {
            _logger.LogInformation($"SUCCESS: Request processed successfully - Method: {context.Request.Method}, Path: '{context.Request.Path}', Status Code: {context.Response.StatusCode}");
        }

        private void LogError(HttpContext context, int statusCode, string message, Exception ex)
        {
            _logger.LogError(ex, $"Error processing request - Method: {context.Request.Method}, Path: '{context.Request.Path}', Status Code: {statusCode}", message);
        }
    }
}