using API.Models;
using Application.ApplicationException;
using Domain.DomainException;
using FSA.LaboratoryManagement.Authorization;
using System.Text.Json;

namespace API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        #region Attributes
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        #endregion

        #region Properties
        #endregion

        public GlobalExceptionMiddleware(
            RequestDelegate requestDelegate, ILogger<GlobalExceptionMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        #region Methods
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
                case InvalidRoomProfileAggregateException:
                    _logger.LogWarning(exception, "Bad request: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorCode = "400 - Bad Request";
                    response.Message = exception.Message;
                    break;

                // Domain Layer Exceptions - 400 Bad Request
                case DomainExceptionBase domainEx:
                    _logger.LogWarning(domainEx, "Domain validation error occurred");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.ErrorCode = "400 - Domain Error";
                    response.Message = domainEx.Message;
                    break;

                // Not Found Exceptions - 404 Not Found
                case DeviceProfileNotFound or RoomProfileNotFound or PatientSensorNotFound:
                    _logger.LogWarning(exception, "Resource not found: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.ErrorCode = "404 - Not Found";
                    response.Message = exception.Message;
                    break;

                // Conflict Exceptions - 409 Conflict
                case PatientExisted:
                    _logger.LogWarning(exception, "Resource conflict: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.ErrorCode = "409 - Conflict";
                    response.Message = exception.Message;
                    break;

                // Authentication/Authorization Exceptions - 401 Unauthorized
                case AuthorizationFailedException:
                    _logger.LogWarning(exception, "Authentication/Authorization error");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.ErrorCode = "401 - Unauthorized";
                    response.Message = exception.Message;
                    break;

                // Application Layer Exceptions - 500 Internal Server Error
                case ApplicationExceptionBase:
                    _logger.LogWarning(exception, "Application error: {ExceptionType}", exception.GetType().Name);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.ErrorCode = "500 - Application Error";
                    response.Message = exception.Message;
                    break;

                // Default Exception - 500 Internal Server Error
                default:
                    _logger.LogError(exception, "An unexpected error occurred");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.ErrorCode = "500 - Internal Server Error";
                    response.Message = $"An internal error occurred. Please try again later, " +
                        $"error detail: {exception.ToString()}";
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
        #endregion
    }
}
