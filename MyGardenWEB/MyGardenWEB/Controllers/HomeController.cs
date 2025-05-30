using Microsoft.AspNetCore.Mvc;
using MyGardenWEB.Models;
using System.Diagnostics;

namespace MyGardenWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IndexProduct()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult ForUs()
        {
            return View();
        }
        public IActionResult Categories()
        {
            return View();
        }
        public IActionResult Photos()
        {
            return View();
        }
        public IActionResult Promotions()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
