using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ship_convenient.Entities.Config
{
    public class TransactionPackageConfig : IEntityTypeConfiguration<TransactionPackage>
    {
        public void Configure(EntityTypeBuilder<TransactionPackage> builder)
        {
            builder.ToTable("TransactionPackage");
        }
    }
}
