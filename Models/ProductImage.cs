namespace Pronia.Models
{
    public class ProductImage: BaseEntity
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public Product Product { get; set; }

        public int ProductId { get; set; }
    }
}
