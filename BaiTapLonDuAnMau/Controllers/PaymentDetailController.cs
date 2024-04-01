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
    public class PaymentDetailController : BaseController
    {
        private readonly BTLDAM _context;

        public PaymentDetailController(BTLDAM context)
        {
            _context = context;
        }

        // GET: PaymentDetail
        public async Task<IActionResult> Index()
        {
            var bTLDAM = _context.PaymentDetail.Include(p => p.Payment).Include(p => p.RoomService);
            return View(await bTLDAM.ToListAsync());
        }

        // GET: PaymentDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PaymentDetail == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetail
                .Include(p => p.Payment)
                .Include(p => p.RoomService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentDetail == null)
            {
                return NotFound();
            }

            return View(paymentDetail);
        }

        // GET: PaymentDetail/Create
        public IActionResult Create()
        {
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentMethod");
            ViewData["RoomServiceId"] = new SelectList(_context.RoomService, "Id", "Id");
            return View();
        }

        // POST: PaymentDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PaymentId,RoomServiceId,Quantity,TotalAmount")] PaymentDetail paymentDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentMethod", paymentDetail.PaymentId);
            ViewData["RoomServiceId"] = new SelectList(_context.RoomService, "Id", "Id", paymentDetail.RoomServiceId);
            return View(paymentDetail);
        }

        // GET: PaymentDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PaymentDetail == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetail.FindAsync(id);
            if (paymentDetail == null)
            {
                return NotFound();
            }
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentMethod", paymentDetail.PaymentId);
            ViewData["RoomServiceId"] = new SelectList(_context.RoomService, "Id", "Id", paymentDetail.RoomServiceId);
            return View(paymentDetail);
        }

        // POST: PaymentDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentId,RoomServiceId,Quantity,TotalAmount")] PaymentDetail paymentDetail)
        {
            if (id != paymentDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentDetailExists(paymentDetail.Id))
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
            ViewData["PaymentId"] = new SelectList(_context.Payment, "Id", "PaymentMethod", paymentDetail.PaymentId);
            ViewData["RoomServiceId"] = new SelectList(_context.RoomService, "Id", "Id", paymentDetail.RoomServiceId);
            return View(paymentDetail);
        }

        // GET: PaymentDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PaymentDetail == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetail
                .Include(p => p.Payment)
                .Include(p => p.RoomService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentDetail == null)
            {
                return NotFound();
            }

            return View(paymentDetail);
        }

        // POST: PaymentDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PaymentDetail == null)
            {
                return Problem("Entity set 'BTLDAM.PaymentDetail'  is null.");
            }
            var paymentDetail = await _context.PaymentDetail.FindAsync(id);
            if (paymentDetail != null)
            {
                _context.PaymentDetail.Remove(paymentDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentDetailExists(int id)
        {
          return (_context.PaymentDetail?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
