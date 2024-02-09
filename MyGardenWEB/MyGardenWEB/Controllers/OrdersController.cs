using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGardenWEB.Data;

namespace MyGardenWEB.Controllers
{
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
                var myGardenDbContext = _context.Orders.
                    Include(o => o.Clients).
                    Include(o => o.Products);
                return View(await myGardenDbContext.ToListAsync());
            }
            else
            {
                var currentUser = _userManager.GetUserId(User);
                var myOrders = _context.Orders.Include(o => o.Products)
                    .Include(u => u.Clients)
                    .Where(x => x.ClientsId == currentUser.ToString()).ToListAsync();
                return View(await myOrders);
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
               .Include(o => o.Products)
               .Include(o => o.Clients)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Orders/Create
        [Authorize(Roles ="User,Admin")]
        public IActionResult Create()
        {
            ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "FirstName","LastName");
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName");
            return View();
        }




        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductsId,ClientsId,Quantity")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "Id", order.ProductsId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "Id", order.ProductsId);
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
                    _context.Update(order);
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
            ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "Id", order.ProductsId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Clients)
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
            var order = await _context.Orders
                .Include(u => u.Clients)
                .FirstOrDefaultAsync(x => x.Id == id);
                //FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
