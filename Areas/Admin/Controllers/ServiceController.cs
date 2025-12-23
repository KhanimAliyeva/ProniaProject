using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController(AppDbContext _context) : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Service service)
        {
            if (!ModelState.IsValid)
                return View(service);

            _context.Services.Add(service);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [HttpPost]
        public IActionResult Update(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }
            var existingService = _context.Services.Find(service.Id);
            if (existingService == null)
            {
                return NotFound();
            }

            existingService.Title = service.Title;
            existingService.Description = service.Description;
            existingService.IconUrl = service.IconUrl;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
