using EntitySecurity.Contract.Security;
using EntitySecurity.Domain;
using EntitySecurity.Logic;
using EntitySecurity.Logic.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ResourceTracker.Api.Middleware;
using ResourceTracker.Application;
using ResourceTracker.Persistence;
using ResourceTracker.Persistence.Data;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddKeyPerFile("/secrets", optional: true, reloadOnChange: true);

        // Register necessary services
        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigureApplicationServices();
        builder.Services.AddEntitySecurity();
        builder.Services.AddScoped<IInfoSetter, InfoSetter>();
        builder.Services.ConfigurePersistenceServices((DbContextOptionsBuilder options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString"));
        }, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. 
                          Enter 'Bearer' [space] and then your token in the text input below.
                          Example: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Resource Tracker Api",
            });
        });

        var app = builder.Build();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
);
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<UserIdentifierMiddleware>();

        app.MapControllers()
            .RequireAuthorization();

        ApplyDbMigrations(app);

        app.Run();
    }


    internal static void ApplyDbMigrations(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();

        if (serviceScope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>().Database.GetPendingMigrations().Count() > 0)
            serviceScope.ServiceProvider.GetRequiredService<ResourceTrackerDbContext>().Database.Migrate();
    }
}