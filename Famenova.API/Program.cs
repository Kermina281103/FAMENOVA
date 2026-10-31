
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using famenova.Infrastructure;
using Famenova.Application;
using Famenova.API.Middleware;
namespace Famenova.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddControllers();
           
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
        {
            new() { Url = "/" }
        };

                    return Task.CompletedTask;
                });
            });
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            var app = builder.Build();
            app.UseCors("AngularPolicy");
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

               
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithOpenApiRoutePattern("/openapi/v1.json");
                });





            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
