namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductGetVM
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string SKU { get; set; }

        public string CategoryName { get; set; }

        public string BrandName { get; set; }

        public string? MainImageUrl { get; set; }

        public string? HoverImageUrl { get; set; }

        public int Rating { get; set; }

        public List<string> Tags { get; set; } = new();

        public List<string> ProductImageUrls { get; set; } = new();

    }
}
