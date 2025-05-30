﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
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

        public IActionResult EmptyCart()
        {
            // Изтриване на данните от базата данни
            var cartItems = _context.Orders;
            _context.Orders.RemoveRange(cartItems);
            _context.SaveChanges();

            // Изчистване на сесията или друг механизъм, който използвате за съхранение на кошницата в паметта
            HttpContext.Session.Remove("Order");
            _context.Orders.RemoveRange(cartItems);

            // Пренасочване към началната страница на кошницата или друга страница по ваш избор
            return RedirectToAction("Index", "Home");
        }
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

                // Заменете цената във ViewData с цената от всяка поръчка
                ViewData["Price"] = await myGardenDbContext.SumAsync(o => o.Price * o.Quantity);

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

        public async Task<IActionResult> CreateWithProductId(int productId, int countP, decimal percent)
        {
            // var currentPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == productId );
            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == productId);
            Order order = new Order();
            //order.ProductsId = productId;
            // productId = order.ProductsId;
            order.ProductsId = productId;
            order.Quantity = countP;
            order.ClientsId = _userManager.GetUserId(User);
            decimal price = 0;
            
            

            if (percent == 100)
            {
                price = Math.Round(order.Quantity * currentProduct.Price, 2);
            }
            else
            {
                price = Math.Round(currentProduct.Price - currentProduct.Price / 100 * percent, 2);
            }
            // }
            order.Price = price; // Запишете цената в поръчката
            _context.Orders.Add(order);
            OrderDetail detail = new OrderDetail();
            detail.ProductsId = order.ProductsId;
            detail.OrderedOn = DateTime.Now;
            detail.Quantity = order.Quantity;
            detail.ClientsId = _userManager.GetUserId(User);
            detail.Total = countP * currentProduct.Price;
            detail.Final = true;
            _context.Add(detail);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            TempData["Price"] = price.ToString();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> CreateProm(int productId, int countP, decimal percent)
        {
            Order order = new Order();
            order.ProductsId = productId;
            order.Quantity = countP;
            order.ClientsId = _userManager.GetUserId(User);
            order.Price = percent;
            //order.RegisterOn = DateTime.Now;
            _context.Orders.Add(order);
            OrderDetail detail = new OrderDetail();
            detail.ProductsId = order.ProductsId;
            detail.OrderedOn = DateTime.Now;
            detail.Quantity = order.Quantity;
            detail.ClientsId = _userManager.GetUserId(User);
            detail.Total = 0;
            detail.Final = true;
            _context.Add(detail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateWithPromotionId(int promotionId, int countP, decimal percent)
        {
            var currentPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId);

            if (currentPromotion == null)
            {
                // Обработка на грешка, ако промоцията не е намерена
                return NotFound();
            }

            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == currentPromotion.ProductsId);

            if (currentProduct == null)
            {
                // Обработка на грешка, ако продуктът не е намерен
                return NotFound();
            }

            decimal price = 0;

            if (percent == 100)
            {
                price = Math.Round(countP * currentProduct.Price, 2);
            }
            else
            {
                price = Math.Round(countP * currentProduct.Price - countP * currentProduct.Price / 100 * percent, 2);
            }

            return await CreateProm(currentPromotion.ProductsId, countP, price);
        }


        public async Task<IActionResult> CreateWithProductsId(int productId, int countP, int percent)
        {
            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == productId);
            Order order = new Order();
            order.ProductsId = productId;
            order.Quantity = 1;
            order.ClientsId = _userManager.GetUserId(User);
            order.Price = currentProduct.Price; // Запишете цената в поръчката

            _context.Orders.Add(order);

            OrderDetail detail = new OrderDetail();
            detail.ProductsId = order.ProductsId;
            detail.OrderedOn = DateTime.Now;
            detail.Quantity = order.Quantity;
            detail.ClientsId = _userManager.GetUserId(User);
            detail.Total = countP * currentProduct.Price;
            detail.Final = true;
            _context.Add(detail);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateWithPromotionsId(int promotionId, int countP, int percent)
        {
            var currentPromotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == promotionId);
            countP = 2;
            if (currentPromotion == null)
            {
                // Обработка на грешка, ако промоцията не е намерена
                return NotFound();
            }

            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == currentPromotion.ProductsId);

            if (currentProduct == null)
            {
                // Обработка на грешка, ако продуктът не е намерен
                return NotFound();
            }

            decimal price = 0;

            if (percent == 100)
            {
                price = Math.Round(countP * currentProduct.Price, 2);
            }
            else
            {
                price = Math.Round(countP * currentProduct.Price - countP * currentProduct.Price / 100 * percent, 2);
            }

            return await CreateProm(currentPromotion.ProductsId, countP, price);
        }
        // GET: Orders/Create
        //[Authorize(Roles ="User,Admin")]
        public IActionResult Create()
        {
            // ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "FirstName", "LastName");
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName");
            return View();
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
                var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == order.ProductsId);
                order.ClientsId = _userManager.GetUserId(User);
                order.Price = order.Quantity * currentProduct.Price;
                _context.Orders.Add(order);
                OrderDetail detail = new OrderDetail();
                detail.ProductsId = order.ProductsId;
                detail.OrderedOn = DateTime.Now;
                detail.Quantity = order.Quantity;
                detail.ClientsId = _userManager.GetUserId(User);
                detail.Total = 0;
                detail.Final = false;
                _context.Add(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName", order.ProductsId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderPromotion([Bind("ProductsId,Quantity")] int productId, Order order, Product product, Promotion prom)
        {
            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == productId);
            // var prodPrice = await _context.Products.FindAsync(productId);
            // prodPrice.Price=product.Price - product.Price / 100 * prom.PromotionPercent;
            order.ProductsId = productId;
            // order.RegisterOn=DateTime.Now;
            order.Quantity = 1;
            order.ClientsId = _userManager.GetUserId(User);
            // order.Products.Price = prod;
            _context.Orders.Add(order);
            OrderDetail detail = new OrderDetail();
            detail.ProductsId = order.ProductsId;
            detail.OrderedOn = DateTime.Now;
            detail.Quantity = order.Quantity;
            detail.ClientsId = _userManager.GetUserId(User);
            detail.Total = 0;
            detail.Final = false;
            _context.Add(detail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "BulgarianName", order.ProductsId);
            //ViewData["ClientsId"] = new SelectList(_context.Users, "Id", "Id", order.ClientsId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductsId,ClientsId,Quantity")] Order order)
        {
            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == order.ProductsId);
            if (id != order.Id)
            {
                return NotFound();
            }
            var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.ProductsId == order.ProductsId);

            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion == null)
                    {
                        order.ClientsId = _userManager.GetUserId(User);
                        order.Price = currentProduct.Price * order.Quantity;
                    }
                    else
                    {
                        order.ClientsId = _userManager.GetUserId(User);
                        order.Price = Math.Round(order.Quantity * currentProduct.Price - order.Quantity * currentProduct.Price / 100 * promotion.PromotionPercent, 2);
                    }
                    //order.ClientsId = _userManager.GetUserId(User);
                    //order.Price = currentProduct.Price * order.Quantity;
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
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MyGardenDbContext.Orders' is null");
            }
            var orders = await _context.Orders.FindAsync(id);
            //.Include(u => u.Clients)
            //.FirstOrDefaultAsync(x => x.Id == id);
            //FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }
            // _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            // return View();
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
