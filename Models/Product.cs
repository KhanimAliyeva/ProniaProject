
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Pronia.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public string SKU { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public int BrandId { get; set; }

    public Brand? Brand { get; set; }

    public string? MainImageUrl { get; set; }

    public string? HoverImageUrl { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public ICollection<ProductTag> ProductTags { get; set; } = [];

    public ICollection<ProductImage> ProductImages { get; set; } = [];

    //public int BrandId { get; set; }

    //public Brand? Brand { get; set; }



}

