
using Bdb.Curso.Application.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Net;
 

namespace Bdb.Curso.Infraestructure
{
    public class ExceptionHandlingMiddlewareSL
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddlewareSL(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception using both Serilog and ILogger
                Log.Error(ex, "Unhandled exception occurred");
                _logger.LogError(ex, "Unhandled exception occurred");

                // Set the response properties
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Create a response model
                var response = new ErrorResponse
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    Detail = ex.Message // Consider using a more secure approach in production
                };

                // Serialize and write the response
                var jsonResponse = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

    }
}
