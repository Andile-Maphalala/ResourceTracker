using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ResourceTracker.Application.Interfaces;
using ResourceTracker.Application.Models.Identity;
using ResourceTracker.Domain.Entities;
using ResourceTracker.Persistence.Data;
using ResourceTracker.Persistence.QueryBuilders.Implementations;
using ResourceTracker.Persistence.QueryBuilders.Interfaces;
using ResourceTracker.Persistence.Repositories;
using System.Text;


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
            services.AddScoped<IGameQueryBuilder, GameQueryBuilder>();
            services.AddScoped<IComponetQueryBuilder, ComponetQueryBuilder>();
            services.AddScoped<IQuestQueryBuilder, QuestQueryBuilder>();
            services.AddScoped<IQuestComponetsQueryBuilder, QuestComponetsQueryBuilder>();
            services.AddScoped<IBuildPlanComponentBuilder, BuildPlanComponentBuilder>();

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
