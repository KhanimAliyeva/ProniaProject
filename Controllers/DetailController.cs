using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using Pronia.ViewModels.ProductViewModels;
using Microsoft.EntityFrameworkCore;

namespace Pronia.Controllers;

public class DetailController : Controller
{
    AppDbContext context;

    public DetailController(AppDbContext _context)
    {
        context = _context;
    }

    public IActionResult Index(int id)
    {
        if (id <= 0)
            return BadRequest();

        var product = context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                SKU = p.SKU,
                Rating = p.Rating,

                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,

                MainImageUrl = p.MainImageUrl,
                HoverImageUrl = p.HoverImageUrl,

                ProductImages = p.ProductImages
                    .Select(pi => pi.ImageUrl)
                    .ToList(),

                Tags = p.ProductTags
                    .Select(pt => pt.Tag.Name)
                    .ToList()
            })
            .FirstOrDefault(x=>x.Id==id);

        if (product == null)
            return NotFound();

        return View(product);
    }
}
