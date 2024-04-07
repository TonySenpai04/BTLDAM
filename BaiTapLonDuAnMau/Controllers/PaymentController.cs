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
    public class PaymentController : BaseController
    {
        private readonly BTLDAM _context;

        public PaymentController(BTLDAM context)
        {
            _context = context;
        }
		[HttpPost]
		public async Task<IActionResult> SubmitPayment(string paymentMethod, DateTime paymentDate, decimal totalAmount,int roomId, List<int> selectedServices)
		{
			try
			{
                Payment payment;
                if (paymentMethod == "payOnArrivalSection")
                {
                    payment = new Payment
                    {
                        PaymentMethod = paymentMethod,
                        PaymentDate = paymentDate,
                        IsPaid = false,
                        Deposit = 0,
                        TotalAmount = totalAmount,
                        RoomId = roomId,

                    };


                }
                else
                {
                    payment = new Payment
                    {
                        PaymentMethod = paymentMethod,
                        PaymentDate = paymentDate,
                        IsPaid = true,
                        Deposit = 0,
                        TotalAmount = totalAmount,
                        RoomId = roomId,

                    };
                }
				_context.Payment.Add(payment);

				await _context.SaveChangesAsync();
				int paymentId = payment.Id;
				foreach (var serviceId in selectedServices)
				{
					var roomService = await _context.RoomService.FindAsync(serviceId);
					if (roomService != null)
					{
						var paymentDetail = new PaymentDetail
						{
							PaymentId = paymentId,
							RoomServiceId = serviceId,
							Quantity = selectedServices.Count,
							TotalAmount = totalAmount
						};

						_context.PaymentDetail.Add(paymentDetail);
					}
					else
					{
						// Xử lý trường hợp serviceId không hợp lệ nếu cần
						// Ví dụ: Hiển thị thông báo lỗi, ghi log, v.v.
					}
				}

				await _context.SaveChangesAsync();



                return RedirectToAction("Index", "Home");


            }
            catch (Exception ex)
			{
				return BadRequest($"Booking failed: {ex.Message}");
				
			}
		}

		[HttpGet]
        [Route("CheckOut")]
        public IActionResult Payment(decimal totalAmount)
        {
				ViewBag.TotalAmount = totalAmount; 
				return View("~/Views/Payment/Payment.cshtml");
		}
        // GET: Payment
        public async Task<IActionResult> Index()
        {
            if (IsLogin)
            {
                var bTLDAM = _context.Payment.Include(p => p.Room);
                return View(await bTLDAM.ToListAsync());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
         
        }

        // GET: Payment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Payment == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payment/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber");
            return View();
        }

        // POST: Payment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalAmount,PaymentDate,PaymentMethod,IsPaid,RoomId,Deposit")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", payment.RoomId);
            return View(payment);
        }

        // GET: Payment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Payment == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", payment.RoomId);
            return View(payment);
        }

        // POST: Payment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TotalAmount,PaymentDate,PaymentMethod,IsPaid,RoomId,Deposit")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", payment.RoomId);
            return View(payment);
        }

        // GET: Payment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Payment == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Payment == null)
            {
                return Problem("Entity set 'BTLDAM.Payment'  is null.");
            }
            var payment = await _context.Payment.FindAsync(id);
            if (payment != null)
            {
                _context.Payment.Remove(payment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
          return (_context.Payment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
