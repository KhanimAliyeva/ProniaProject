using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia.Configurations
{
    public class SliderConfiguration : IEntityTypeConfiguration<Slider>
    {
        public void Configure(EntityTypeBuilder<Slider> builder)
        {
            builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).IsRequired().HasMaxLength(256);
            builder.Property(s => s.DiscountPercentage).IsRequired();
            builder.ToTable(opt =>
            {
                opt.HasCheckConstraint("CK_Slider_DiscountPercentage", "[DiscountPercentage] >= 0 AND [DiscountPercentage] <= 100");
            });
            builder.Property(s => s.ImageUrl).HasMaxLength(512);
        }
    }
}
