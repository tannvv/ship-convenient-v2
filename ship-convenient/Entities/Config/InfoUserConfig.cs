using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class InfoUserConfig : IEntityTypeConfiguration<InfoUser>
    {
        public void Configure(EntityTypeBuilder<InfoUser> builder)
        {
            builder.ToTable("InfoUser");
            builder.HasIndex(info => info.Phone).IsUnique();
            builder.HasIndex(info => info.Email).IsUnique();
            builder.HasMany(info => info.Routes)
                 .WithOne(route => route.InfoUser).HasForeignKey(route => route.InfoUserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(info => info.Vehicles)
                .WithOne(vehi => vehi.InfoUser).HasForeignKey(vehi => vehi.InfoUserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
