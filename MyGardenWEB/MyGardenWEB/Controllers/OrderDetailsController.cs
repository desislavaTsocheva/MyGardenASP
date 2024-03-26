using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyGardenWEB.Data;
using NuGet.Protocol.Core.Types;

namespace MyGardenWEB.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly MyGardenDbContext _context;
        private readonly UserManager<Client> _userManager;
        public const string OrderSessionKey = "OrderId";
        public List<Order> orderList = new List<Order>();
        public List<OrderDetail> detailsList = new List<OrderDetail>();

        public OrderDetailsController(MyGardenDbContext context, UserManager<Client> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize]
        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var myGardenDbContext = _context.OrderDetail
                   .Include(o => o.Clients)
                   .Include(o => o.Products);
                return View(await myGardenDbContext.ToListAsync());
            }
            else
            {
                var myGardenDbContext = _context.OrderDetail
                    .Include(o => o.Clients)
                    .Include(o => o.Products)
                    .Where(x => x.ClientsId == _userManager.GetUserId(User));
                return View(await myGardenDbContext.ToListAsync());
            }
        }
        [NonAction]
        public int? GetOrdertId()
        {
            return HttpContext.Session.GetInt32("OrderSessionKey");
        }
        public async Task<IActionResult> Calculate(int orderId)
        {
            var currentUser = _userManager.GetUserId(User);
            var dbOrderList = _context.OrderDetail
               .Include(p => p.Products)
               .Include(o => o.Products.Orders)
               .Where(x => (x.Id == orderId) &&
               (x.Final == false) &&
               (x.ClientsId == currentUser));
            decimal sum = 0;
            foreach (var item in dbOrderList)
            {
                sum += (item.Products.Price * item.Quantity);
            }
            //започва актуализиране на таблицата Orders /total=....; final=true
            OrderDetail order = await _context.OrderDetail.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Final = true;
            order.Total = sum;

            _context.OrderDetail.Update(order);
            await _context.SaveChangesAsync();
            //изтрива ОРДЕРид от сесията
            HttpContext.Session.Remove("OrderSessionKey");
            TempData["OrderActive"] = false;

            TempData["Message"] = "Успешно поръчахте на стойност " + sum.ToString();
            return RedirectToAction("Index", "Products");

        }

        //public async Task<IActionResult> FilterByDate(DateTime date)
        //{
        //    var filteredOrders = await _context.OrderDetail
        //               .Where(o => o.OrderedOn.Date == date.Date)
        //               .ToListAsync();

        //    return View(filteredOrders);
        //}
        public IActionResult FilterByDate(DateTime date)
        {
            var orders = _context.OrderDetail
                .Include(o=>o.Products)
                .Include(o=>o.Clients)
                .Where(o => o.OrderedOn.Date == date.Date).ToList();
            ViewData["Date"] = date.ToShortDateString(); // Показване на избраната дата в заглавието на изгледа
            return View(nameof(Index),orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            TempData.Keep();

            if (!ModelState.IsValid)
            {
                ViewData["ProductsId"] = new SelectList(_context.Products, "Id", "Id");
                return View();

            }
            if (GetOrdertId() == null)
            {
                Order order = new Order()
                {
                    ClientsId = _userManager.GetUserId(User),
                    Quantity = 1,
                    ProductsId= product.Id
                    
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSessionKey", order.Id);
                TempData["Message"] = "You order sucssesfully";
                TempData["OrderActive"] = true;
            }
            int shoppingCardId = (int)GetOrdertId();
            var orderItem = await _context.OrderDetail
                .SingleOrDefaultAsync(x => (x.ProductsId == product.Id));
            if (orderItem == null) //Ако поръчва друг/нов продукт се записва в OrderDetails
            {
                orderItem = new OrderDetail()
                {
                    ProductsId = product.Id,
                    Id = (int)GetOrdertId()
                };
                _context.OrderDetail.Add(orderItem);
            }
            else //ако избира поръчан вече продукт се увеличава количеството му
            {

                _context.OrderDetail.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            //return Content("OK");
            return RedirectToAction("Index", "Products"); //??? къде да се върнем?
        }


        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.Clients)
                .Include(o => o.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetail.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.Id == id);
        }
    }
}

