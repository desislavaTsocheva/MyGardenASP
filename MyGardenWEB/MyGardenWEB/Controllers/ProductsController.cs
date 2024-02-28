using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyGardenWEB.Data;
using MyGardenWEB.Models;
using MyGardenWEB.Services;
using static NuGet.Packaging.PackagingConstants;

namespace MyGardenWEB.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyGardenDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string wwwroot;

        public ProductsController(MyGardenDbContext context)
        {
            _context = context;
        }
        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            //var myGardenDbContext = _context.Products.Include(p => p.Categories);
            //return View(await _context.Products.Where(x => x.CategoriesId == 1).ToListAsync());
            //List<Product> model = await _context.Products.Include(img => img.Photos).ToListAsync();
            //foreach(var item in model)
            //{
            //    item.Photos=_context.Photos.Where(x=>x.ProductsId==item.Id).ToList();
            //}

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
            //var myGardenDbContext = _context.Products.Include(p => p.Categories);
            // return View(await _context.Products.Where(x => x.CategoriesId == 2 || x.CategoriesId == 3).ToListAsync());
           
            //List<Product> model = await _context.Products.Include(img => img.Photos).ToListAsync();
            //foreach (var item in model)
            //{
            //    item.Photos = _context.Photos.Where(x => x.ProductsId == item.Id).ToList();
            //}

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
            //Product product = await _context.Products.Include(img => img.Photos).FirstOrDefaultAsync(m => m.Id == id);
            //var imagePath = Path.Combine(wwwroot, "Photos");
            //ProductDetails modelVM = new ProductDetails()
            //{
            //    BulgarianName=product.BulgarianName,
            //    LatinName=product.LatinName,
            //    Size=product.Size,  
            //    Description=product.Description,    
            //    Price=product.Price, 
            //    Categories=product.Categories,
            //    ImagesPaths = _context.Photos
            //    .Where(img => img.ProductsId == product.Id)
            //    .Select(x => $"/ProductImages/{x.Url}").ToList<string>()
            //};

            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BulgarianName,LatinName,Size,Description,PhotoURL,Price,CategoriesId")] Product product)
        {
            product.RegisterOn = DateTime.Now;
            if (!ModelState.IsValid)
            {
                ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
                return View(product);

            }
            //ImagesBuilder imagesBuilder = new ImagesBuilder(_context, _hostEnvironment);
            //await imagesBuilder.CreateImages(products);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            //ProductDetails model = new ProductDetails
            //{
            //    Id = product.Id,
            //    BulgarianName = product.BulgarianName,
            //    LatinName = product.LatinName,  
            //    Size = product.Size,
            //    Price = product.Price,
            //    Description = product.Description,
            //    Categories = product.Categories,
            //    ImagesPaths = _context.Photos
            //   .Where(img => img.ProductsId == product.Id)
            //   .Select(x => $"/ProductImages/{x.Url}").ToList<string>()
            //};
            //return View(model);
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
            //product.RegisterOn = DateTime.Now;
            //modelToDB.BulgarianName = product.BulgarianName;
            //modelToDB.LatinName = product.LatinName;
            //modelToDB.Size = product.Size;
            //modelToDB.Price = product.Price;
            //modelToDB.Description = product.Description;
            //modelToDB.Categories = product.Categories;

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
