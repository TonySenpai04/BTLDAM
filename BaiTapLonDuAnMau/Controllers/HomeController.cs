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
        [HttpGet]
        [Route("Statistics")]
        public IActionResult Statistics()
        {
            if (IsLogin && string.Compare(ViewBag.IsLogin, "1", true) == 0)
            {
                int totalBookings = _context.Bookings.Count();


                var staffBookingCounts = _context.Bookings
                                            .GroupBy(b => b.StaffId)
                                            .Select(g => new
                                            {
                                                StaffId = g.Key,
                                                StaffName = g.First().Staff.FullName,
                                                BookingCount = g.Count()
                                            })
                                            .OrderByDescending(x => x.BookingCount)
                                            .ToList();


                ViewData["TotalBookings"] = totalBookings;
                ViewBag.StaffBookingCounts = staffBookingCounts;

                return View("Statistics");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

           
        }





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