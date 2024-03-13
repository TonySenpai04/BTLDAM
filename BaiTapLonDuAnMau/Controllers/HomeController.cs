using BaiTapLonDuAnMau.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaiTapLonDuAnMau.Controllers
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
        [HttpGet]
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        //[HttpGet]
        //[Route("Service")]
        //public IActionResult Service()
        //{
        //    return View();
        //}

       

       

        [HttpGet]
        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        [Route("Testimonial")]
        public IActionResult Testimonial()
        {
            return View();
        }
        public IActionResult Privacy()
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