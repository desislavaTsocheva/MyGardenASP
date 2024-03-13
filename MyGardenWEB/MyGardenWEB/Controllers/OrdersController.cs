using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGardenWEB.Data;

namespace MyGardenWEB.Controllers
{
    [Authorize] 
    public class OrdersController : Controller
    {
        private readonly MyGardenDbContext _context;
        private readonly UserManager<Client> _userManager;

        public OrdersController(MyGardenDbContext context, UserManager<Client> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var myGardenDbContext = _context.Orders
                   .Include(o => o.Clients)
                   .Include(o => o.Products);
                return View(await myGardenDbContext.ToListAsync());
            }
            else
            {
                var myGardenDbContext = _context.Orders
                    .Include(o => o.Clients)
                    .Include(o => o.Products)
                    .Where(x => x.ClientsId == _userManager.GetUserId(User));
                return View(await myGardenDbContext.ToListAsync());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
               .Include(o => o.Clients)
               .Include(o => o.Products)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Orders/Create
        //[Authorize(Roles ="User,Admin")]
        public IActionResult Create()
        {
           // ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "FirstName", "LastName");
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName");
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWithProductId([Bind("ProductsId,Quantity")] int productId, int countP)
        {
            Order order = new Order();
            //order.ProductsId = productId;
            productId = order.ProductsId;
            order.Quantity = countP;
            order.ClientsId = _userManager.GetUserId(User);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductsId,Quantity")] Order order)
        {
            if (ModelState.IsValid)
            {
               // order.RegisterOn = DateTime.Now;
                order.ClientsId = _userManager.GetUserId(User);
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName", order.ProductsId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            //ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName", order.ProductsId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductsId,ClientsId,Quantity")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.ClientsId = _userManager.GetUserId(User);
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            //ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName", order.ProductsId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Clients)
                .Include(o => o.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MyGardenDbContext.Orders' is null");
            }
            var order = await _context.Orders.FindAsync(id);
            //.Include(u => u.Clients)
            //.FirstOrDefaultAsync(x => x.Id == id);
            //FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            // _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
