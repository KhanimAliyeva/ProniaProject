using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).IsRequired().HasMaxLength(500);
            builder.Property(s => s.IconUrl).HasMaxLength(255);
        }
    }
}
