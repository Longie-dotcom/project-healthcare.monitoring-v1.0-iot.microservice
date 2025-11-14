using API.Models;
using Application.ApplicationException;
using Domain.DomainException;
using HCM.Authorization;
using System.Text.Json;

namespace API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate requestDelegate, ILogger<GlobalExceptionMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "Application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                // Domain Layer Exceptions - 400 Bad Request
                case InvalidUserAggregateException or
                InvalidRoleAggregateException or
                InvalidPasswordOVException or InvalidChangePassword:
                    _logger.LogWarning(exception, "Bad request: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Type = "Bad Request";
                    response.Message = exception.Message;
                    break;

                // Domain Layer Exceptions - 400 Bad Request
                case DomainExceptionBase domainEx:
                    _logger.LogWarning(domainEx, "Domain validation error occurred");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Type = "Domain Error";
                    response.Message = domainEx.Message;
                    break;

                // Not Found Exceptions - 404 Not Found
                case UserNotFound or UserEmailNotFound or RoleCodeNotFound 
                    or RoleGuidNotFound or PrivilegeNotFound:
                    _logger.LogWarning(exception, "Resource not found: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Type = "Not Found";
                    response.Message = exception.Message;
                    break;

                // Conflict Exceptions - 409 Conflict
                case UserAlreadyExists or RoleCodeAlreadyExists or PrivilegeAlreadyExists:
                    _logger.LogWarning(exception, "Resource conflict: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.Type = "Conflict";
                    response.Message = exception.Message;
                    break;

                // Authentication/Authorization Exceptions - 401 Unauthorized
                case InvalidTokenException or InvalidResetPassword 
                or InvalidRole or InvalidPassword or AuthorizationFailedException:
                    _logger.LogWarning(exception, "Authentication/Authorization error");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Type = "Unauthorized";
                    response.Message = exception.Message;
                    break;

                // Application Layer Exceptions - 500 Internal Server Error
                case ApplicationExceptionBase:
                    _logger.LogWarning(exception, "Application error: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Type = "Application Error";
                    response.Message = exception.Message;
                    break;

                // Default Exception - 500 Internal Server Error
                default:
                    _logger.LogError(exception, "An unexpected error occurred");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Type = "Internal Server Error";
                    response.Message = "An internal error occurred. Please try again later.";
                    response.Details = exception.ToString();
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
