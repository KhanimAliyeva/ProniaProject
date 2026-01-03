using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using Pronia.Helpers;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModels;
using Pronia.ViewModels.Slider;

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
        List<SliderGetVM> slider = _context.Sliders.Select(s => new SliderGetVM
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            DiscountPercentage = s.DiscountPercentage,
            ImageUrl = s.ImageUrl
        }).ToList();
        return View(slider);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(SliderCreateVM vm)
    {
        if (vm == null)
        {
            return NotFound();

        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (vm.ImageFile == null || !vm.ImageFile.CheckType("image/"))
        {
            ModelState.AddModelError("ImageFile", "Image is required and must be an image file");
            return View(vm);
        }
        if (!vm.ImageFile.CheckSize(2))
        {
            ModelState.AddModelError("ImageFile", "Image size must be less than 2MB");
            return View(vm);
        }


        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
        string imageFileName = vm.ImageFile.SaveFile(folderPath);
        Slider slider = new Slider
        {
            Title = vm.Title,
            Description = vm.Description,
            DiscountPercentage = vm.DiscountPercentage,
            ImageUrl = imageFileName
        };
        _context.Sliders.Add(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    public IActionResult Update(int? id)
    {
        Slider? slider = _context.Sliders.Find(id);
        if (slider == null)
        {
            return NotFound();
        }
        SliderUpdateVM vm = new SliderUpdateVM
        {
            Title = slider.Title,
            Description = slider.Description,
            DiscountPercentage = slider.DiscountPercentage,
            ExistingImageUrl = slider.ImageUrl
        };
        return View(vm);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int id, SliderUpdateVM vm)
    {
        Slider? slider = _context.Sliders.Find(id);
        if (slider == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.ImageFile != null)
        {
            if (!vm.ImageFile.CheckType("image/"))
            {
                ModelState.AddModelError("ImageFile", "Only image files are allowed");
                return View(vm);
            }

            if (!vm.ImageFile.CheckSize(2))
            {
                ModelState.AddModelError("ImageFile", "Image size must be less than 2MB");
                return View(vm);
            }

            string folderPath = Path.Combine( _environment.WebRootPath,"assets","images","website-images");

            string oldImagePath = Path.Combine(folderPath, slider.ImageUrl);
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);

            string newImageName = vm.ImageFile.SaveFile(folderPath);
            slider.ImageUrl = newImageName;
        }

        slider.Title = vm.Title;
        slider.Description = vm.Description;
        slider.DiscountPercentage = vm.DiscountPercentage;

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var slider = _context.Sliders.Find(id);
        if (slider == null)
        {
            return NotFound();
        }
        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
        if (!string.IsNullOrEmpty(slider.ImageUrl))
        {
            string mainPath = Path.Combine(folderPath, slider.ImageUrl);
            if (System.IO.File.Exists(mainPath))
                System.IO.File.Delete(mainPath);
        }

        _context.Sliders.Remove(slider);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }




}