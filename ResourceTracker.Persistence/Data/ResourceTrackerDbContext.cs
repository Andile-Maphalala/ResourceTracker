using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;


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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourceTrackerDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ComponentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.QuestComponentsConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.QuestConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RecipeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RecipeComponentConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
