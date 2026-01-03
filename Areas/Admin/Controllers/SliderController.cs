using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public SliderController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var sliders = _context.Sliders.ToList();
        return View(sliders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Slider slider)
    {
        if (!ModelState.IsValid)
            return View(slider);

        if (slider.ImageFile == null)
        {
            ModelState.AddModelError("ImageFile", "Image is required");
            return View(slider);
        }

        if (!slider.ImageFile.ContentType.StartsWith("image/"))
        {
            ModelState.AddModelError("ImageFile", "Only image files are allowed");
            return View(slider);
        }

        if (slider.ImageFile.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("ImageFile", "Image size must be max 2MB");
            return View(slider);
        }

        if (slider.DiscountPercentage < 0 || slider.DiscountPercentage > 100)
        {
            ModelState.AddModelError("DiscountPercentage", "Discount must be between 0 and 100");
            return View(slider);
        }

        string folderPath = Path.Combine(
            _environment.WebRootPath,
            "assets",
            "images",
            "website-images"
        );

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = Guid.NewGuid() + "_" + slider.ImageFile.FileName;
        string filePath = Path.Combine(folderPath, fileName);

        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            slider.ImageFile.CopyTo(fs);
        }

        slider.ImageUrl = $"assets/images/website-images/{fileName}";

        _context.Sliders.Add(slider);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
            return NotFound();

        string fullPath = Path.Combine(_environment.WebRootPath, slider.ImageUrl);

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);

        _context.Sliders.Remove(slider);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
            return NotFound();

        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(Slider slider)
    {
        if (!ModelState.IsValid)
            return View(slider);

        var existingSlider = _context.Sliders.Find(slider.Id);
        if (existingSlider == null)
            return NotFound();

        if (slider.DiscountPercentage < 0 || slider.DiscountPercentage > 100)
        {
            ModelState.AddModelError("DiscountPercentage", "Discount must be between 0 and 100");
            return View(slider);
        }

        existingSlider.Title = slider.Title;
        existingSlider.Description = slider.Description;
        existingSlider.DiscountPercentage = slider.DiscountPercentage;

        if (slider.ImageFile != null)
        {
            if (!slider.ImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("ImageFile", "Only image files are allowed");
                return View(slider);
            }

            if (slider.ImageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "Image size must be max 2MB");
                return View(slider);
            }

            string oldPath = Path.Combine(_environment.WebRootPath, existingSlider.ImageUrl);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            string folderPath = Path.Combine(
                _environment.WebRootPath,
                "assets",
                "images",
                "website-images"
            );

            string newFileName = Guid.NewGuid() + "_" + slider.ImageFile.FileName;
            string newFilePath = Path.Combine(folderPath, newFileName);

            using (FileStream fs = new FileStream(newFilePath, FileMode.Create))
            {
                slider.ImageFile.CopyTo(fs);
            }

            existingSlider.ImageUrl = $"assets/images/website-images/{newFileName}";
        }

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
