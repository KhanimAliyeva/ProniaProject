using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.HasMany(t => t.ProductTags)
                   .WithOne(pt => pt.Tag)
                   .HasForeignKey(pt => pt.TagId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
