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
    public class HotelInfoController : BaseController
    {
        private readonly BTLDAM _context;

        public HotelInfoController(BTLDAM context)
        {
            _context = context;
        }

        // GET: HotelInfo
        public async Task<IActionResult> Index()
        {
            if (IsLogin && string.Compare(ViewBag.IsLogin, "1", true) == 0)
            {
                return _context.HotelInfo != null ?
                        View(await _context.HotelInfo.ToListAsync()) :
                        Problem("Entity set 'BTLDAM.HotelInfo'  is null.");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
          
         
        }
        [HttpGet]
        public IActionResult GetHotelInfo()
        {
            var hotelInfo = _context.HotelInfo.FirstOrDefault(); 
            return Json(hotelInfo); 
        }

        // GET: HotelInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HotelInfo == null)
            {
                return NotFound();
            }

            var hotelInfo = await _context.HotelInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelInfo == null)
            {
                return NotFound();
            }

            return View(hotelInfo);
        }

        // GET: HotelInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HotelInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HotelName,PhoneNumber,Email,Address,FbLink,YtLink,TwLink,InLink")] HotelInfo hotelInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotelInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotelInfo);
        }

        // GET: HotelInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HotelInfo == null)
            {
                return NotFound();
            }

            var hotelInfo = await _context.HotelInfo.FindAsync(id);
            if (hotelInfo == null)
            {
                return NotFound();
            }
            return View(hotelInfo);
        }

        // POST: HotelInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HotelName,PhoneNumber,Email,Address,FbLink,YtLink,TwLink,InLink")] HotelInfo hotelInfo)
        {
            if (id != hotelInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotelInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelInfoExists(hotelInfo.Id))
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
            return View(hotelInfo);
        }

        // GET: HotelInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HotelInfo == null)
            {
                return NotFound();
            }

            var hotelInfo = await _context.HotelInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelInfo == null)
            {
                return NotFound();
            }

            return View(hotelInfo);
        }

        // POST: HotelInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HotelInfo == null)
            {
                return Problem("Entity set 'BTLDAM.HotelInfo'  is null.");
            }
            var hotelInfo = await _context.HotelInfo.FindAsync(id);
            if (hotelInfo != null)
            {
                _context.HotelInfo.Remove(hotelInfo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelInfoExists(int id)
        {
          return (_context.HotelInfo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
