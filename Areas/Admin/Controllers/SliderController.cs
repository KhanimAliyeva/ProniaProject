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

    [HttpGet]
    public IActionResult Update(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
        {
            return NotFound();
        }
        return View(slider);
    }

    [HttpPost]
    public IActionResult Update(Slider slider)
    {
        if (!ModelState.IsValid)
        {
            return View(slider);
        }
        var existingSlider = _context.Sliders.Find(slider.Id);
        if (existingSlider == null)
        {
            return NotFound();
        }
        if (slider.DiscountPercentage < 0 || slider.DiscountPercentage > 100)
        {
            ModelState.AddModelError("DiscountPercentage", "Discount Percentage must be between 0 and 100.");
            return View(slider);
        }
        existingSlider.Title = slider.Title;
        existingSlider.Description = slider.Description;
        existingSlider.DiscountPercentage = slider.DiscountPercentage;
        existingSlider.ImageUrl = slider.ImageUrl;
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}

