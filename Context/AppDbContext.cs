

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace Pronia.Context
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }
    }
}
