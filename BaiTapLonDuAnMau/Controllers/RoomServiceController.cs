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
    public class RoomServiceController : BaseController
    {
        private readonly BTLDAM _context;

        public RoomServiceController(BTLDAM context)
        {
            _context = context;
        }

        // GET: RoomService
        public async Task<IActionResult> Index()
        {
            if (IsLogin )
            {
                var bTLDAM = _context.RoomService.Include(r => r.Booking).Include(r => r.Room).Include(r => r.Service);
                return View(await bTLDAM.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        // GET: RoomService/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RoomService == null)
            {
                return NotFound();
            }

            var roomService = await _context.RoomService
                .Include(r => r.Booking)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roomService == null)
            {
                return NotFound();
            }

            return View(roomService);
        }

        // GET: RoomService/Create
        public IActionResult Create()
        {
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ID", "ServiceName");
            return View();
        }

        // POST: RoomService/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingId,RoomId,ServiceId")] RoomService roomService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "FullName", roomService.BookingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", roomService.RoomId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ID", "ServiceName", roomService.ServiceId);
            return View(roomService);
        }

        // GET: RoomService/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RoomService == null)
            {
                return NotFound();
            }

            var roomService = await _context.RoomService.FindAsync(id);
            if (roomService == null)
            {
                return NotFound();
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "FullName", roomService.BookingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", roomService.RoomId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ID", "ServiceName", roomService.ServiceId);
            return View(roomService);
        }

        // POST: RoomService/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingId,RoomId,ServiceId")] RoomService roomService)
        {
            if (id != roomService.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomServiceExists(roomService.Id))
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
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "FullName", roomService.BookingId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", roomService.RoomId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ID", "ServiceName", roomService.ServiceId);
            return View(roomService);
        }

        // GET: RoomService/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RoomService == null)
            {
                return NotFound();
            }

            var roomService = await _context.RoomService
                .Include(r => r.Booking)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roomService == null)
            {
                return NotFound();
            }

            return View(roomService);
        }

        // POST: RoomService/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RoomService == null)
            {
                return Problem("Entity set 'BTLDAM.RoomService'  is null.");
            }
            var roomService = await _context.RoomService.FindAsync(id);
            if (roomService != null)
            {
                _context.RoomService.Remove(roomService);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomServiceExists(int id)
        {
          return (_context.RoomService?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
