namespace Pronia.ViewModels.Slider
{
    public class SliderCreateVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DiscountPercentage { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
