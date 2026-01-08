using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.ToTable(opt =>
            {
                opt.HasCheckConstraint("CK_Product_Price", "[Price] >= 0");
            });

            builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.HasIndex(p => p.SKU).IsUnique();

            builder.Property(p => p.Rating).IsRequired();
            builder.ToTable(opt =>
            {

            opt.HasCheckConstraint("CK_Product_Rating", "[Rating] >= 1 AND [Rating] <= 5");
            });

            builder.Property(p => p.MainImageUrl).HasMaxLength(255);
            builder.Property(p => p.HoverImageUrl).HasMaxLength(255);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                     .HasForeignKey(p => p.BrandId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProductTags)
                   .WithOne(pt => pt.Product)
                   .HasForeignKey(pt => pt.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProductImages)
                     .WithOne(pi => pi.Product)
                     .HasForeignKey(pi => pi.ProductId)
                     .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
