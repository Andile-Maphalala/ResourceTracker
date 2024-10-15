using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResourceTracker.Application.Repositories;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Data;
using ResourceTracker.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using ResourceTracker.Application.Models.Identity;


namespace ResourceTracker.Persistence
{
    public static class PersistenceServicesResgistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureContext, IConfiguration configuration)
        {
            services.AddDbContext<ResourceTrackerDbContext>(configureContext);
            services.AddScoped<IResourceTrackerRepository, ResourceTrackerRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ResourceTrackerDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                };
            });


            return services;
        }

        public static Action<DbContextOptionsBuilder> ConfigureDbContext(string connectionstring)
        {
            return (DbContextOptionsBuilder options) =>
            {
                options.UseSqlServer(connectionstring);
            };
        }
    }
}
