using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models;

public class Slider: BaseEntity
{


    public string Title { get; set; }

    public string Description { get; set; }

    public int DiscountPercentage { get; set; }

    public string? ImageUrl { get; set; }

    
}
