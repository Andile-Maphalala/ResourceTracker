using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceTracker.Domain.Entities;

namespace ResourceTracker.Persistence.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                 new Role
                 {
                     Id = 1,
                     Name = "Administrator",
                     NormalizedName = "ADMINISTRATOR",
                     ConcurrencyStamp = "ADMIN_ROLE"
                 },
                new Role
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "USER_ROLE"
                }
               
            );
        }
    }
}
