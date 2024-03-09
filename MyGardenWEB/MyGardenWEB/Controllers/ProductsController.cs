using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyGardenWEB.Data;
using MyGardenWEB.Models;
using MyGardenWEB.Services;
using MyGardenWEB.ViewModel;
using static NuGet.Packaging.PackagingConstants;

namespace MyGardenWEB.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyGardenDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        ///private string wwwroot;

        public ProductsController(MyGardenDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
        }
        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {

            if (searchString.IsNullOrEmpty())
            {
                return View(await _context.Products.Where(x => x.CategoriesId == 1).ToListAsync());
            }
            if (_context.Products == null)
            {
                return Problem("Context is empty");
            }
            var products = from m in _context.Products select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.BulgarianName.Contains(searchString)||s.LatinName.Contains(searchString));
            }
            return View(products.ToList()); //return View(await _context.Flowers.ToListAsync());
        }
        public async Task<IActionResult> IndexProduct(string searchString)
        {
            if (searchString.IsNullOrEmpty())
            {
              return View(await _context.Products.Where(x => x.CategoriesId == 2 || x.CategoriesId == 3).ToListAsync());
            }
            if (_context.Products == null)
            {
                return Problem("Context is empty");
            }
            var products = from m in _context.Products select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.BulgarianName.Contains(searchString)||s.LatinName.Contains(searchString));
            }
            return View(products.ToList()); //return View(await _context.Flowers.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var photos=await _context.Photos.Include(p=>p.Files).ToListAsync();

            var products = await _context.Products
                .Include(p => p.Categories)
                .Include(h=>h.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (products == null)
            {
                return NotFound();
            }
            ProductVM prodVM = new ProductVM()
            {
                BulgarianName = products.BulgarianName,
                LatinName = products.LatinName,
                Size = products.Size,
                Price = products.Price,
                PhotoURL = products.PhotoURL,
                Description = products.Description,
                Files = _context.Photos
                .Where(img => img.ProductsId == products.Id)
                .Select(x => $"~/Files/{x.Url}").ToList<string>()
            };


            return View(prodVM);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            //Photo images = new Photo();
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BulgarianName,LatinName,Size,Description,PhotoURL,Price,CategoriesId")] Product product,Photo photo)
        {
            product.RegisterOn = DateTime.Now;
            if (!ModelState.IsValid)
            {
                ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
                return View(product);

            }
            
           await _context.SaveChangesAsync();
           
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private string Upload(IFormFile file)
        {
            string fileName = "";
            if (file != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, file.Name);
                fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }
        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BulgarianName,LatinName,Size,Description,PhotoURL,Price,CategoriesId")] Product product)
        {
            //Product modelToDB = await _context.Products.FindAsync(id);
            if (id != product.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
                return View(product);
            }
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
