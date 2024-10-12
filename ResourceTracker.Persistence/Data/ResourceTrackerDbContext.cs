using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResourceTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceTracker.Persistence.Data
{
    public partial class ResourceTrackerDbContext : DbContext
    {
        public ResourceTrackerDbContext(DbContextOptions<ResourceTrackerDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Quest> Quests { get; set; }
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<QuestComponents> QuestComponents { get; set; }
        public virtual DbSet<ResourceCollection> ResourceCollections { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourceTrackerDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ComponentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.QuestComponentsConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.QuestConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RecipeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ResourceCollectionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ResourceConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
