using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string SKU { get; set; }
        
        public string? MainImageUrl { get; set; }

        public IFormFile? MainImageFile { get; set; }

        public string? HoverImageUrl { get; set; }

        public IFormFile? HoverImageFile { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Tag section cannot be null.")]
        public List<int>? TagIds { get; set; } = [];

        [Required(ErrorMessage = "Category cannot be null.")]
        public int CategoryId { get; set; }

        public int BrandId { get; set; }    

        public List<IFormFile>? ProductImageFiles { get; set; } = [];

        public List<string>? ProductImageUrls { get; set; } = [];

        public List<int>? ProductImageIds { get; set; } = [];




    }
}
