using EntitySecurity.Contract.Security;
using EntitySecurity.Logic;
using EntitySecurity.Logic.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ResourceTracker.Api.Middleware;
using ResourceTracker.Application;
using ResourceTracker.Application.Common.Behavior;
using ResourceTracker.ImageStorageService;
using ResourceTracker.Persistence;
using ResourceTracker.Persistence.Data;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddKeyPerFile("/secrets", optional: true, reloadOnChange: true);

        // Register necessary services
        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureImageStorageServices();
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
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Resource Tracker Api",
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
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
        app.UseMiddleware<CustomExceptionHandlerMiddleware>();

        app.MapControllers()
            .RequireAuthorization();

        app.UseStaticFiles();

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