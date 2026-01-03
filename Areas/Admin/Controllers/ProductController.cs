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

        private void SendItemsWithViewBag()
        {
            ViewBag.Categories = _context.Categories.ToList();

            ViewBag.Tags = _context.Tags.ToList();

            ViewBag.Brands = _context.Brands.ToList();

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
            SendItemsWithViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateVM vm)
        {
            var ratingFromForm = Request.Form["Rating"];

            if (!ModelState.IsValid)
            {
                SendItemsWithViewBag();
                return View(vm);
            }
            if (string.IsNullOrEmpty(ratingFromForm))
            {
                ModelState.AddModelError("Rating", "The Rating field is required.");
                SendItemsWithViewBag();
                return View(vm);
            }

            if (!_context.Categories.Any(c => c.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendItemsWithViewBag();
                return View(vm);
            }

            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    if (!_context.Tags.Any(t => t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", "One or more selected tags are not valid");
                        SendItemsWithViewBag();
                        return View(vm);
                    }
                }
            }
            

            if (vm.MainImageFile == null || !vm.MainImageFile.CheckType("image/"))
            {
                ModelState.AddModelError("MainImageFile", "Main image is required and must be an image file");
                SendItemsWithViewBag();
                return View(vm);
            }

            if (!vm.MainImageFile.CheckSize(2))
            {
                ModelState.AddModelError("MainImageFile", "Main image size must be less than 2MB");
                SendItemsWithViewBag();
                return View(vm);
            }

            if (vm.HoverImageFile == null || !vm.HoverImageFile.CheckType("image/"))
            {
                ModelState.AddModelError("HoverImageFile", "Hover image is required and must be an image file");
                SendItemsWithViewBag();
                return View(vm);
            }

            if (!vm.HoverImageFile.CheckSize(2))
            {
                ModelState.AddModelError("HoverImageFile", "Hover image size must be less than 2MB");
                SendItemsWithViewBag();
                return View(vm);
            }

            if(vm.BrandId == 0 || !_context.Brands.Any(b => b.Id == vm.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand is not valid");
                SendItemsWithViewBag();
                return View(vm);
            }
           

            foreach (var imageFile in vm.ProductImageFiles)
            {
                if (!imageFile.CheckType("image/"))
                {
                    ModelState.AddModelError("ProductImageFiles", "All product images must be image files");
                    SendItemsWithViewBag();
                    return View(vm);
                }
                if (!imageFile.CheckSize(2))
                {
                    ModelState.AddModelError("ProductImageFiles", "All product images size must be less than 2MB");
                    SendItemsWithViewBag();
                    return View(vm);
                }
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
                Rating = vm.Rating,
                ProductTags = [],
                BrandId= vm.BrandId,

            };

            foreach (var imageFile in vm.ProductImageFiles)
            {
                string productImageFileName = imageFile.SaveFile(folderPath);
                ProductImage productImage = new ProductImage
                {
                    ImageUrl = productImageFileName,
                    Product = product
                };
                product.ProductImages.Add(productImage);
            }

            foreach (var tagid in vm.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagid
                };
                product.ProductTags.Add(productTag);


            }

            _context.Products.Add(product);
            _context.SaveChanges();



            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductTags).FirstOrDefault(x => x.Id == id);
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
                Rating = product.Rating,
                BrandId= product.BrandId,
                TagIds = product.ProductTags.Select(p => p.TagId).ToList(),
                ProductImageUrls = product.ProductImages.Select(p => p.ImageUrl).ToList(),
                ProductImageIds = product.ProductImages.Select(p => p.Id).ToList()



            };

            SendItemsWithViewBag();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendItemsWithViewBag();
                return View(vm);
            }

            if (!_context.Categories.Any(c => c.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendItemsWithViewBag();
                return View(vm);
            }
            if(vm.BrandId == 0 || !_context.Brands.Any(b => b.Id == vm.BrandId))
            {
                ModelState.AddModelError("BrandId", "Brand is not valid");
                SendItemsWithViewBag();
                return View(vm);
            }

            var existProduct = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductTags)
                .FirstOrDefault(x => x.Id == vm.Id);

            if (existProduct == null)
                return NotFound();

            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
            //main image
            if (vm.MainImageFile != null)
            {
                if (!vm.MainImageFile.CheckType("image/") || !vm.MainImageFile.CheckSize(2))
                {
                    ModelState.AddModelError("MainImageFile", "Invalid main image");
                    SendItemsWithViewBag();
                    return View(vm);
                }

                string newMainImage = vm.MainImageFile.SaveFile(folderPath);

                if (!string.IsNullOrEmpty(existProduct.MainImageUrl))
                {
                    string oldPath = Path.Combine(folderPath, existProduct.MainImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existProduct.MainImageUrl = newMainImage;
            }

            //tag
            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    if (!_context.Tags.Any(t => t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", "One or more selected tags are not valid");
                        SendItemsWithViewBag();
                        return View(vm);
                    }
                }
            }

            //hover image
            if (vm.HoverImageFile != null)
            {
                if (!vm.HoverImageFile.CheckType("image/") || !vm.HoverImageFile.CheckSize(2))
                {
                    ModelState.AddModelError("HoverImageFile", "Invalid hover image");
                    SendItemsWithViewBag();
                    return View(vm);
                }

                string newHoverImage = vm.HoverImageFile.SaveFile(folderPath);

                if (!string.IsNullOrEmpty(existProduct.HoverImageUrl))
                {
                    string oldPath = Path.Combine(folderPath, existProduct.HoverImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existProduct.HoverImageUrl = newHoverImage;
            }

            existProduct.Name = vm.Name;
            existProduct.Description = vm.Description;
            existProduct.Price = vm.Price;
            existProduct.SKU = vm.SKU;
            existProduct.CategoryId = vm.CategoryId;
            existProduct.Rating = vm.Rating;
            existProduct.BrandId = vm.BrandId;
            existProduct.ProductTags = [];

            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    existProduct.ProductTags.Add(new ProductTag
                    {
                        TagId = tagId
                    });
                }
            }

            if (vm.ProductImageFiles != null)
            {
                foreach (var imageFile in vm.ProductImageFiles)
                {
                    if (!imageFile.CheckType("image/"))
                    {
                        ModelState.AddModelError("ProductImageFiles", "All product images must be image files");
                        SendItemsWithViewBag();
                        return View(vm);
                    }

                    if (!imageFile.CheckSize(2))
                    {
                        ModelState.AddModelError("ProductImageFiles", "All product images size must be less than 2MB");
                        SendItemsWithViewBag();
                        return View(vm);
                    }
                }
            }

            if (vm.ProductImageIds != null)
            {
                foreach (var productImage in existProduct.ProductImages.ToList())
                {
                    if (!vm.ProductImageIds.Contains(productImage.Id))
                    {
                        existProduct.ProductImages.Remove(productImage);

                        string oldPath = Path.Combine(folderPath, productImage.ImageUrl);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                }
            }

            if (vm.ProductImageFiles != null)
            {
                foreach (var imageFile in vm.ProductImageFiles)
                {
                    string productImageFileName = imageFile.SaveFile(folderPath);

                    existProduct.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = productImageFileName
                    });
                }
            }

            _context.Products.Update(existProduct);
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

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
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
                    Rating = p.Rating,
                    BrandName = p.Brand.Name,
                    Tags = p.ProductTags.Select(pt => pt.Tag.Name).ToList(),
                    ProductImageUrls = p.ProductImages.Select(pi => pi.ImageUrl).ToList()
                })
                .FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
