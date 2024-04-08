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

        //[HttpGet]
        //public async Task<IActionResult> CreateWithProductsId([Bind("ProductsId,Quantity")] int productId, int countP)
        //{
        //    var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == productId);
        //    var currentProducts = await _context.Orders.Include(o => o.Products).FirstOrDefaultAsync(m => m.Id == productId);
        //    Product product = new Product();
        //    Order order = new Order();
        //    //order.ProductsId = productId;
        //    // productId = order.ProductsId;
        //    order.ProductsId = product.Id;
        //    order.Quantity = countP;
        //    order.ClientsId = _userManager.GetUserId(User);
        //    var price = countP * currentProduct.Price;
        //    _context.Orders.Add(order);

        //    OrderDetail detail = new OrderDetail();
        //    detail.ProductsId = order.ProductsId;
        //    detail.OrderedOn = DateTime.Now;
        //    detail.Quantity = order.Quantity;
        //    detail.ClientsId = _userManager.GetUserId(User);
        //    detail.Total = countP * currentProduct.Price;
        //    detail.Final = true;
        //    _context.Add(detail);


        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}



        [HttpPost]
        public async Task<IActionResult> CreateWithProductId([Bind("ProductsId,Quantity")] int productId, int countP)
        {
            var currentProduct = await _context.Products.FirstOrDefaultAsync(z => z.Id == productId);
            Order order = new Order();
            //order.ProductsId = productId;
            // productId = order.ProductsId;
            order.ProductsId = productId;
            order.Quantity = 1;
            order.ClientsId = _userManager.GetUserId(User);
            var price = countP * currentProduct.Price;
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
                order.ClientsId = _userManager.GetUserId(User);
                _context.Orders.Add(order);
                OrderDetail detail=new OrderDetail();
                detail.ProductsId=order.ProductsId;
                detail.OrderedOn=DateTime.Now;
                detail.Quantity=order.Quantity;
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
        //    public static OrderDetail GetCart(IServiceProvider services)
        //    {
        //        //get acsses to the session
        //        ISession session = services.GetRequiredService<IHttpContextAccessor>().HttpContext.Session;
        //        var context = services.GetService<MyGardenDbContext>();
        //        //check if we have the stored id in the session
        //        string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
        //        //set the id value to the session
        //        session.GetString("CartId");
        //        //create order detail object, include context and id session
        //        return new OrderDetail(context)
        //        {
        //            Id = cartId
        //        };
        //    }
        //    public void AddToCart(Product product, int amount)
        //    {
        //        var cartItem = _context.OrderDetails.Include(x => x._context.Products)
        //        .SingleOrDefault(s => s.Product.Id == product.Id && s.Id == Id);
        //        if (cartItem == null)
        //        {
        //            cartItem = new Order
        //            {
        //                Id = cartItem.Id,
        //                Products = cartItem.Products,
        //                Quantity = 1
        //            };
        //            _context.OrderDetails.Add(cartItem);
        //        }
        //        else
        //        {
        //            cartItem.Amount++;
        //        }
        //        _context.SaveChanges();
        //    }
        //    public void RemoveFromCart(Product product)
        //    {
        //        var cartItem = _context.OrderDetails.Include(x => x._context.Products)
        //        .SingleOrDefault(s => s.Product.Id == product.Id && s.Id == Id);
        //        var localAmount = 0;
        //        if (cartItem != null)
        //        {
        //            if (cartItem.Amount > 1)
        //            {
        //                cartItem.Amount--;
        //                localAmount = cartItem.Amount;
        //            }
        //        }
        //        else
        //        {
        //            _context.OrderDetails.Remove(cartItem);
        //        }
        //        _context.SaveChanges();
        //        return localAmount;
        //    }
        //    public List<Order> GetCartItems()
        //    {
        //        return OrderDetail ?? (OrderDetail == _context.OrderDetails.Where(c => c.Id == Id)
        //            .Include(s => s.Product).ToList());
        //    }
        //    public decimal GetTotal()
        //    {
        //        var total = _context.OrderDetails.Where(c => c.Id == Id).Select(c => c.Product.Price * c.Quantity).Sum();
        //        return total; 
        //    }
        //}
    }
}
