using Application.Activities;
using Application.Core;
using Infrastructure.Interfaces;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions;

public static class AppServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
        });
        
        services.AddControllers();
        
        services.AddDbContext<DataContext>(options =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string connStr;

            if (env == "Development")
            {
                connStr = config.GetConnectionString("defaultConnection");
            }
            else
            {
                var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL")!;

                connUrl = connUrl.Replace("postgres://", string.Empty);

                var pgUserPass = connUrl.Split("@")[0];
                var pgHostPortDb = connUrl.Split("@")[1];

                var pgHostPort = pgHostPortDb.Split("/")[0];
                var pgDb = pgHostPortDb.Split("/")[1];

                var pgUser = pgUserPass.Split(":")[0];
                var pgPass = pgUserPass.Split(":")[1];
                var pgHost = pgHostPort.Split(":")[0];
                var pgPort = pgHostPort.Split(":")[1];

                connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
            }
            
            options.UseNpgsql(connStr);
        });
        
        services.AddCors(opt => opt.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins(config.GetValue<string>("origin"));
        }));

        services.AddMediatR(typeof(List.Handler).Assembly);
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        services.AddSignalR();
        
        services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IImageAccessor, ImageAccessor>();
    }
}