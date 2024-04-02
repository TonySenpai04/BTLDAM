using BaiTapLonDuAnMau.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaiTapLonDuAnMau.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BTLDAM _context;

        public HomeController(ILogger<HomeController> logger, BTLDAM context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms.ToListAsync();
            ViewBag.RoomsHome = rooms.Take(3).ToList();

            var staff = await _context.Staff.ToListAsync();
            ViewBag.StaffHome = staff.Take(4).ToList();
            var services = await _context.Service.ToListAsync();
            ViewBag.ServiceHome = services.Take(3).ToList();
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