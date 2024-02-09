using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGardenWEB.Data;

namespace MyGardenWEB.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyGardenDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string wwwroot;

        public ProductsController(MyGardenDbContext context,IWebHostEnvironment hostEnvironment, string wwwroot)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            this.wwwroot = $"{this._hostEnvironment.WebRootPath}";
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            List<Product> model = await _context.Products.Include(img => img.Photos).ToListAsync();
            foreach (var item in model)
            {
                item.Photos = _context.Photos.Where(x => x.ProductsId == item.Id).ToList();

            }
            return View(model); 
            //var myGardenDbContext = _context.Products.Include(p => p.Categories);
            //return View(await myGardenDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(img => img.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var imagePath = Path.Combine(wwwroot, "Photos");


            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            //ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BulgarianName,LatinName,Size,Description,PhotoURL,Price,RegisterOn,CategoriesId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.RegisterOn=DateTime.Now;    
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null||_context.Products==null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            //ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BulgarianName,LatinName,Size,Description,PhotoURL,Price,RegisterOn,CategoriesId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.RegisterOn=DateTime.Now;    
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
            //ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null||_context.Products==null)
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
            if (_context.Products == null)
            {
                return Problem("Entity set 'MyGardenDbContext.Products' is null. ");
            }

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
