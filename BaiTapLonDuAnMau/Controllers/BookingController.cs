using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTapLonDuAnMau.Models;
using NuGet.Protocol.Plugins;

namespace BaiTapLonDuAnMau.Controllers
{
    public class BookingController : BaseController
    {
        private readonly BTLDAM _context;

        public BookingController(BTLDAM context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("BookingRoom")]

        public async Task<IActionResult> BookingView()
        {
            var rooms = await _context.Rooms.ToListAsync();
            ViewBag.Rooms = rooms;
            List<Service> services = await _context.Services.ToListAsync();
            ViewBag.Services = services;

            return View("Booking");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookingView(int roomId)
        {
            List<Room> room = await _context.Rooms.Where(r => r.Id == roomId).ToListAsync();
            if (room == null)
            {
                return NotFound();
            }
            ViewBag.RoomId = roomId;
            List<Service> services = await _context.Services.ToListAsync();
            ViewBag.Services = services;

            return View("Booking", room);
        }

       
        [HttpPost]
        public async Task<IActionResult> BookNow(string fullName, string phoneNumber, string email,
            DateTime checkIn, DateTime checkOut, int numAdults, int numChildren, int roomId, string specialRequests, List<int> selectedServices)
        {
            try
            {
                if (checkOut <= checkIn)
                {
                    return BadRequest("Check-out time must be greater than check-in time.");
                }
                TimeSpan duration = checkOut - checkIn;
                int hours = (int)duration.TotalHours;

                // Tạo đối tượng Booking
                var booking = new Booking
                {
                    RoomId = roomId,
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    NumAdults = numAdults,
                    NumChildren = numChildren,
                    Status = "Chờ duyệt",
                    SpecialRequests = specialRequests,
                    Email = email,
                    StaffId = null,
                };

                // Lưu thông tin đặt phòng vào cơ sở dữ liệu
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Cập nhật trạng thái phòng và tính tổng số tiền
                var room = await _context.Rooms.FindAsync(roomId);
                decimal roomTotalAmount = 0;
                if (room != null)
                {
                    room.Status = "Đã đặt-Chờ xác nhận";
                    roomTotalAmount = room.Price * hours;
                }

                // Tính tổng số tiền từ các dịch vụ đã chọn
                decimal serviceTotalAmount = 0;
                if (selectedServices != null && selectedServices.Any())
                {
                    foreach (var serviceId in selectedServices)
                    {
                        var service = await _context.Services.FindAsync(serviceId);
                        if (service != null)
                        {
                            serviceTotalAmount += service.Price;
                        }
                    }
                }
                decimal totalAmount = roomTotalAmount + serviceTotalAmount;

				List<int> selectedServicesId = new List<int>();

				if (selectedServices != null && selectedServices.Any())
                {
                    foreach (var serviceId in selectedServices)
                    {
                        var roomService = new RoomService
                        {
                            BookingId = booking.Id,
                            RoomId = roomId,
                            ServiceId = serviceId,
						
						};
						

						_context.RoomService.Add(roomService);
						await _context.SaveChangesAsync();

						selectedServicesId.Add(roomService.Id);
					}
                    await _context.SaveChangesAsync();
                }
               

				// Chuyển hướng đến trang CheckOut với thông tin tổng số tiền
				return Json(new { success = true, redirectUrl = Url.Action("CheckOut", new { totalAmount, roomId , selectedServicesId }) });
            }
            catch (Exception ex)
            {
                return BadRequest($"Booking failed: {ex.Message}");
            }
        }

        public IActionResult CheckOut(decimal totalAmount,int roomId,List<int> selectedServicesId)
		{
			ViewBag.TotalAmount = totalAmount;
            ViewBag.RoomId = roomId;
			ViewBag.SelectedServicesId = selectedServicesId;
			return View("~/Views/Payment/Payment.cshtml");
		}
		[HttpPost]
        public async Task<ActionResult> CancelBooking(string roomNumber, string phoneNumber)
        {
            // Kiểm tra xem số phòng và số điện thoại có tồn tại và khớp với dữ liệu trong cơ sở dữ liệu không
            var booking = _context.Bookings.FirstOrDefault(b => b.Room.RoomNumber == roomNumber && b.PhoneNumber == phoneNumber);

            if (booking != null)
            {

                try
                {
                    booking.Status = "Đã hủy";

                    // Xác định nhân viên thực hiện xác nhận
                    var userLogin = await _context.Accounts.FirstOrDefaultAsync(m => m.Username == CurrentUser);
                    if (userLogin != null)
                    {
                        var staff = await _context.Staff.FirstOrDefaultAsync(m => m.Id == userLogin.StaffId);
                        if (staff != null)
                        {
                            booking.StaffId = staff.Id;
                        }
                    }

                    _context.Update(booking);
                    var roomBooking = await _context.Room.FindAsync(booking.RoomId);
                    if (roomBooking != null)
                    {
                        roomBooking.Status = "Trống";
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

              

                return Json(new { success = true, message = "Booking successfully cancelled." });
            }
            else
            {
                // Nếu không tìm thấy đặt phòng, trả về một thông báo lỗi
                return Json(new { success = false, message = "Could not cancel booking. Room number or phone number is incorrect." });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            try
            {
                booking.Status = "Đã duyệt";

                // Xác định nhân viên thực hiện xác nhận
                var userLogin = await _context.Accounts.FirstOrDefaultAsync(m => m.Username == CurrentUser);
                if (userLogin != null)
                {
                    var staff = await _context.Staff.FirstOrDefaultAsync(m => m.Id == userLogin.StaffId);
                    if (staff != null)
                    {
                        booking.StaffId = staff.Id;
                    }
                }

                _context.Update(booking);
                var roomBooking = await _context.Room.FindAsync(booking.RoomId);
                if(roomBooking != null)
                {
                    roomBooking.Status = "Đã duyệt-Chờ nhận phòng";
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.Id))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            try
            {
                booking.Status = "Đã hủy";

                // Xác định nhân viên thực hiện xác nhận
                var userLogin = await _context.Accounts.FirstOrDefaultAsync(m => m.Username == CurrentUser);
                if (userLogin != null)
                {
                    var staff = await _context.Staff.FirstOrDefaultAsync(m => m.Id == userLogin.StaffId);
                    if (staff != null)
                    {
                        booking.StaffId = staff.Id;
                    }
                }

                _context.Update(booking);
                var roomBooking = await _context.Room.FindAsync(booking.RoomId);
                if (roomBooking != null)
                {
                    roomBooking.Status = "Trống";
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.Id))
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
        // GET: Booking
        public async Task<IActionResult> Index()
        {

            if (!IsLogin)
            {
                return RedirectToAction("Login", "Account");
            }

            var bTLDAM = _context.Bookings.Include(b => b.Room);
                return View(await bTLDAM.ToListAsync());
            
           
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber");
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName");
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,PhoneNumber,Email,RoomId,CheckIn,CheckOut,NumAdults,NumChildren,SpecialRequests,Status,StaffId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", booking.StaffId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", booking.StaffId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,PhoneNumber,Email,RoomId,CheckIn,CheckOut,NumAdults,NumChildren,SpecialRequests,Status,StaffId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", booking.StaffId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'BTLDAM.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
          return (_context.Bookings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
