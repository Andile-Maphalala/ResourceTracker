using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ResourceTracker.Domain.Entities;


namespace ResourceTracker.Persistence.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));

            //Key
            builder.HasKey(x => x.Id);

            //Properties
            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.ProfileImage)
           .IsRequired(false);

            //Relationships
            builder.HasMany(x => x.Quests)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
