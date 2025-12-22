using Microsoft.AspNetCore.Mvc;
using Pronia.Context;

namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController(AppDbContext _context) : Controller
{

    [HttpGet]
    public IActionResult Index()
    {
        var sliders = _context.Sliders.ToList();
        return View(sliders);
    }
    [HttpPost]
    public IActionResult Create(Slider slider)
    {
        if (!ModelState.IsValid)
        {
            return View(slider);
        }

        if (slider.DiscountPercentage<0 || slider.DiscountPercentage>100)
        {
            ModelState.AddModelError("DiscountPercentage", "Discount Percentage must be between 0 and 100.");
            return View(slider);
        }
        _context.Sliders.Add(slider);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    public IActionResult Delete(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
        {
            return NotFound();
        }
        _context.Sliders.Remove(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}

