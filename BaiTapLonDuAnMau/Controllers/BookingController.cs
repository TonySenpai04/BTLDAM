using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTapLonDuAnMau.Models;

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
            return View("Booking", room);
        }

        [HttpPost]
        public async Task<IActionResult> BookNow(string fullName, string phoneNumber, string email,
            DateTime checkIn, DateTime checkOut, int numAdults, int numChildren, int roomId, string specialRequests)
        {
            try
            {

                // Create a new Booking object
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
                    StaffId=null,
                };

                // Add the new Booking to the context
                _context.Bookings.Add(booking);
                var room = await _context.Rooms.FindAsync(roomId);
                if (room != null)
                {
                    room.Status = "Đã đặt-Chờ xác nhận";
                }
                // Update the status of the room



                await _context.SaveChangesAsync();

                return Ok("Booking successful!");
            }
            catch (Exception ex)
            {

                return BadRequest($"Booking failed: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CancelBooking(string roomNumber, string phoneNumber)
        {
            // Kiểm tra xem số phòng và số điện thoại có tồn tại và khớp với dữ liệu trong cơ sở dữ liệu không
            var booking = _context.Bookings.FirstOrDefault(b => b.Room.RoomNumber == roomNumber && b.PhoneNumber == phoneNumber);

            if (booking != null)
            {
                // Nếu tìm thấy đặt phòng, hủy đặt phòng ở đây (ví dụ: cập nhật trạng thái của đặt phòng)
                //var room = await _context.Rooms.FindAsync(booking.RoomId);
                //if (room != null)
                //{

                //    room.Status = "Trống";
                //}
                //_context.Bookings.Remove(booking);


                //_context.SaveChanges(); 

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
