using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Context;
using Pronia.Helpers;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private void SendCategoriesWithViewBag()
        {
            ViewBag.Categories = _context.Categories.ToList();
        }

        public IActionResult Index()
        {
            List<ProductGetVM> vms = _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductGetVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    SKU = p.SKU,
                    CategoryName = p.Category.Name,
                    MainImageUrl = p.MainImageUrl,
                    HoverImageUrl = p.HoverImageUrl,
                    Rating = p.Rating

                })
                .ToList();

            return View(vms);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SendCategoriesWithViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateVM vm)
        {
            var ratingFromForm = Request.Form["Rating"];

            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (!_context.Categories.Any(c => c.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (vm.MainImageFile == null || !vm.MainImageFile.CheckType("image/"))
            {
                ModelState.AddModelError("MainImageFile", "Main image is required and must be an image file");
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (!vm.MainImageFile.CheckSize(2))
            {
                ModelState.AddModelError("MainImageFile", "Main image size must be less than 2MB");
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (vm.HoverImageFile == null || !vm.HoverImageFile.CheckType("image/"))
            {
                ModelState.AddModelError("HoverImageFile", "Hover image is required and must be an image file");
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (!vm.HoverImageFile.CheckSize(2))
            {
                ModelState.AddModelError("HoverImageFile", "Hover image size must be less than 2MB");
                SendCategoriesWithViewBag();
                return View(vm);
            }
            vm.Rating = int.Parse(Request.Form["Rating"]);

            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

            string mainImageFileName = vm.MainImageFile.SaveFile(folderPath);
            string hoverImageFileName = vm.HoverImageFile.SaveFile(folderPath);

            Product product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                SKU = vm.SKU,
                CategoryId = vm.CategoryId,
                MainImageUrl = mainImageFileName,
                HoverImageUrl = hoverImageFileName,
                Rating = vm.Rating

            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            ProductUpdateVM vm = new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                MainImageUrl = product.MainImageUrl,
                HoverImageUrl = product.HoverImageUrl,
                Rating = product.Rating

            };

            SendCategoriesWithViewBag();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(vm);
            }

            if (!_context.Categories.Any(c => c.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(vm);
            }

            var dbProduct = _context.Products.Find(vm.Id);
            if (dbProduct == null)
                return NotFound();

            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

            if (vm.MainImageFile != null)
            {
                if (!vm.MainImageFile.CheckType("image/") || !vm.MainImageFile.CheckSize(2))
                {
                    ModelState.AddModelError("MainImageFile", "Invalid main image");
                    SendCategoriesWithViewBag();
                    return View(vm);
                }

                string newMainImage = vm.MainImageFile.SaveFile(folderPath);

                if (!string.IsNullOrEmpty(dbProduct.MainImageUrl))
                {
                    string oldPath = Path.Combine(folderPath, dbProduct.MainImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                dbProduct.MainImageUrl = newMainImage;
            }

            if (vm.HoverImageFile != null)
            {
                if (!vm.HoverImageFile.CheckType("image/") || !vm.HoverImageFile.CheckSize(2))
                {
                    ModelState.AddModelError("HoverImageFile", "Invalid hover image");
                    SendCategoriesWithViewBag();
                    return View(vm);
                }

                string newHoverImage = vm.HoverImageFile.SaveFile(folderPath);

                if (!string.IsNullOrEmpty(dbProduct.HoverImageUrl))
                {
                    string oldPath = Path.Combine(folderPath, dbProduct.HoverImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                dbProduct.HoverImageUrl = newHoverImage;
            }

            dbProduct.Name = vm.Name;
            dbProduct.Description = vm.Description;
            dbProduct.Price = vm.Price;
            dbProduct.SKU = vm.SKU;
            dbProduct.CategoryId = vm.CategoryId;
            dbProduct.Rating = vm.Rating;


            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

            if (!string.IsNullOrEmpty(product.MainImageUrl))
            {
                string mainPath = Path.Combine(folderPath, product.MainImageUrl);
                if (System.IO.File.Exists(mainPath))
                    System.IO.File.Delete(mainPath);
            }

            if (!string.IsNullOrEmpty(product.HoverImageUrl))
            {
                string hoverPath = Path.Combine(folderPath, product.HoverImageUrl);
                if (System.IO.File.Exists(hoverPath))
                    System.IO.File.Delete(hoverPath);
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
