using famenova.Infrastructure.Data.Context;
using Famenova.Application.Exceptions;
using Famenova.Application.Responses;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using AppValidationException = Famenova.Application.Exceptions.ValidationException;

namespace Famenova.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IServiceProvider _service;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IWebHostEnvironment environment,IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
            _service = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context,ex);

            }
        }

        public async Task HandleExceptionAsync(HttpContext context,Exception ex)
        {
            await LogExceptionAsync(context, ex);
            ApiResponse response = ex switch
            {
                NotFoundException e =>
                ApiResponse.Failure(e.Message, (int)HttpStatusCode.NotFound),

                UnauthorizedException e =>
                ApiResponse.Failure(e.Message, (int)HttpStatusCode.Unauthorized),

                BadRequestException e =>
                ApiResponse.Failure(e.Message, (int)HttpStatusCode.BadRequest),
                ForbiddenException e =>
                ApiResponse.Failure(e.Message, (int)HttpStatusCode.Forbidden),

                ConflictException e =>
                ApiResponse.Failure(e.Message, (int)HttpStatusCode.Conflict),

                AppValidationException e =>
                ApiValidationResponse.ValidationFail(e.Errors),

                _ => _environment.IsDevelopment()
                    ? ApiResponse.Failure($"{ex.Message} | StackTrace :{ex.StackTrace}", (int)HttpStatusCode.InternalServerError)
                    : ApiResponse.Failure(
                        "An unexpected error Occured !", (int)HttpStatusCode.InternalServerError)






            };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response.ToString());
        }



    public async Task LogExceptionAsync(HttpContext context,Exception ex)
        {
            if (_environment.IsDevelopment())
            {
                //Log to console in development
                _logger.LogError(ex, ex.Message);
            }
            else
            {
                // log to Database in Production 
                using var Scope = _service.CreateScope();
                var dbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await dbContext.LogEntries.AddAsync(new LogEntry
                {
                    Message = ex.Message,
                    
                   
                });
                await dbContext.SaveChangesAsync();

            }
        }

    }
}
