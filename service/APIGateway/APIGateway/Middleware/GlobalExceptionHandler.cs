using Grpc.Core;
﻿using APIGateway.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APIGateway.Middleware
{
    public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException vex)
            {
                logger.LogError(vex, vex.Message);
                await WriteProblemDetailsAsync(context, 400, "Validation Failed", vex.Message);
            }
            catch (UnauthorizedAccessException unauthorized)
            {
                logger.LogError(unauthorized, unauthorized.Message);
                await WriteProblemDetailsAsync(context, 401, "Unauthorized", "Access denied");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, ex.Message);
                await WriteProblemDetailsAsync(context, 404, "Not Found", "Resource not found");
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogError(dbEx, dbEx.Message);
                await WriteProblemDetailsAsync(context, 409, "Database Error", "Conflict occurred while updating the database");
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex, ex.Message);
                await WriteProblemDetailsAsync(context, 408, "Request Timeout", "The operation was cancelled or timed out");
            }
            catch (NotImplementedException ex)
            {
                logger.LogError(ex, ex.Message);
                await WriteProblemDetailsAsync(context, 501, "Not Implemented", "This feature is not yet available");
            }
            catch (BadHttpRequestException badReq)
            {
                logger.LogError(badReq, badReq.Message);
                await WriteProblemDetailsAsync(context, 400, "Bad Request", badReq.Message);
            }
            catch (HttpRequestException httpEx)
            {
                logger.LogError(httpEx, httpEx.Message);
                await WriteProblemDetailsAsync(context, 502, "External Request Failed", httpEx.Message);
            }
            catch (RpcException grpcEx)
            {
                logger.LogError(grpcEx, grpcEx.Message);
                await WriteProblemDetailsAsync(context, 502, "gRPC Error", grpcEx.Status.Detail);
            }

            catch (AppException appEx)
            {
                logger.LogError(appEx, appEx.Message);
                await WriteProblemDetailsAsync(context, appEx.StatusCode, "Application Error", appEx.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                await WriteProblemDetailsAsync(context, 500, "Internal Server Error", "An unexpected error occurred");
            }
        }

        private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.StatusCode = statusCode;
            ProblemDetails problem = new()
            {
                Status = statusCode,
                Type = title,
                Title = title,
                Detail = detail
            };
            string json = JsonSerializer.Serialize(problem);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}

