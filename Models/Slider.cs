using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Slider: BaseEntity
{

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    public string Description { get; set; }

    [Required]
    [Range(0, 100)]
    public int DiscountPercentage { get; set; }

    [Required]
    [MaxLength(512)]
    public string ImageUrl { get; set; }
}
