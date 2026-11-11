
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using famenova.Infrastructure;
using Famenova.Application;
using Famenova.API.Middleware;
using Famenova.Application.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Famenova.API.OpenApi.Famenova.API.OpenApi;
namespace Famenova.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddControllers();
           
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); 

                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
                     {
                        new() { Url = "/" }
                    };

                    return Task.CompletedTask;
                });
            });

            var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            builder.Services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = jwt.Issuer,
                       ValidateAudience = true,
                       ValidAudience = jwt.Audience,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });
            builder.Services.AddAuthorization();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
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
            using (var scope = app.Services.CreateScope())
                await IdentitySeeder.SeedAsync(scope.ServiceProvider, builder.Configuration);
              
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStaticFiles();
            app.UseCors("AngularPolicy");
           
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
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
