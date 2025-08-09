using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ResourceTracker.Persistence.Data.Configurations;


namespace ResourceTracker.Persistence.Data
{
    public partial class ResourceTrackerDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, UserLogin, IdentityRoleClaim<int>, UserToken>
    {
        public ResourceTrackerDbContext(DbContextOptions<ResourceTrackerDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Quest> Quests { get; set; }
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<QuestComponents> QuestComponents { get; set; }
        public virtual DbSet<BuildPlan> BuildPlans { get; set; }
        public virtual DbSet<BuildPlanComponent> BuildPlanComponents { get; set; }
        public virtual DbSet<BuildPlanResource> BuildPlanResources { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourceTrackerDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ComponentConfiguration());
            modelBuilder.ApplyConfiguration(new QuestComponentsConfiguration());
            modelBuilder.ApplyConfiguration(new QuestConfiguration());
            modelBuilder.ApplyConfiguration(new RecipeConfiguration());
            modelBuilder.ApplyConfiguration(new RecipeComponentConfiguration());
            modelBuilder.ApplyConfiguration(new BuildPlanConfiguration());
            modelBuilder.ApplyConfiguration(new BuildPlanComponentConfiguration());
            modelBuilder.ApplyConfiguration(new BuildPlanResourceConfiguration());
            modelBuilder.ApplyConfiguration(new GameConfiguration());
            modelBuilder.ApplyConfiguration(new PictureConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
