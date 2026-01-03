using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public IFormFile? MainImageFile { get; set; }
        public IFormFile? HoverImageFile { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public List<int>? TagIds { get; set; }

        public List<IFormFile>? ProductImageFiles { get; set; }
    }
}