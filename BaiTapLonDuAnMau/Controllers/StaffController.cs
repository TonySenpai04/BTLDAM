using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTapLonDuAnMau.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace BaiTapLonDuAnMau.Controllers
{
    public class StaffController : Controller
    {
        private readonly BTLDAM _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StaffController(BTLDAM context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        [Route("Team")]
        public async Task<IActionResult> Team()
        {
            var staffs = await _context.Staffs.Where(s => s.Position != "Employee").ToListAsync();
            ViewBag.Staffs = staffs;
            return View();
        }
        // GET: Staff
        public async Task<IActionResult> Index()
        {
              return _context.Staffs != null ? 
                          View(await _context.Staffs.ToListAsync()) :
                          Problem("Entity set 'BTLDAM.Staffs'  is null.");
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staff/Create
        public IActionResult Create()
        {
            return View();
        }
        private string Upload(IFormFile formFile)
        {
               var uniqueFileName = "";
               var filePath = "";
               var file = "";
           
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file = "img/" + uniqueFileName;
               using var stream = new FileStream(filePath, FileMode.Create) ;
               formFile.CopyTo(stream);
               return file;

        }

        // POST: Staff/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffViewModel staffViewModel)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = "";
                var filePath = "";
                var file = "";
                if (staffViewModel.Avatar != null && staffViewModel.Avatar.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                    // Tạo tên file độc nhất để tránh trùng lặp
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + staffViewModel.Avatar.FileName;
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    file = "img/" + uniqueFileName;
                    // Lưu file vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await staffViewModel.Avatar.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn của file vào trường Avatar
                    //account.Avatar = "/images/" + uniqueFileName;



                }
                Staff staff = new Staff()
                {
                    FullName = staffViewModel.FullName,
                    Avatar =file,
                    Position = staffViewModel.Position,
                    FbLink = staffViewModel.FbLink,
                    InstagramLink = staffViewModel.InstagramLink,
                    TwLink = staffViewModel.TwLink,
                    Email=staffViewModel.Email,
                    PhoneNumber=staffViewModel.PhoneNumber,
                };
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(staffViewModel);
        }

        // GET: Staff/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            
            IFormFile formFile = null;
            string formfileAString = "";
            string relativePath = staff.Avatar;
            string rootPath = _webHostEnvironment.WebRootPath;

            // Kết hợp đường dẫn root với đường dẫn tương đối của ảnh
            string physicalPath = Path.Combine(rootPath, relativePath.TrimStart('/'));
            // Kiểm tra tệp tin avatar có tồn tại không
            if (System.IO.File.Exists(physicalPath))
            {
                // Mở đọc tệp tin và tạo một đối tượng Stream
                using (var stream = System.IO.File.OpenRead(physicalPath))
                {
                    // Tạo đối tượng IFormFile từ Stream và thông tin tên tệp tin
                    formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(physicalPath));
                    formfileAString = formFile.FileName;
                    // Gán formFile cho thuộc tính Avatar của viewModel

                }
            }
            StaffViewModel staffViewModel = new StaffViewModel()
            {
                FullName = staff.FullName,
                Avatar = formFile,
                Position = staff.Position,
                FbLink = staff.FbLink,
                InstagramLink = staff.InstagramLink,
                TwLink = staff.TwLink,
                Email=staff.Email,
                PhoneNumber=staff.PhoneNumber,  
            };
            return View(staffViewModel);
        }

        // POST: Staff/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffViewModel staffViewModel)
        {
            if (id != staffViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var staff = await _context.Staffs.FindAsync(id);

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    string item = staff.Avatar;
                    var uniqueFileName = "";
                    var filePath = "";

                    // Kiểm tra và lưu ảnh đại diện mới nếu có
                    if (staffViewModel.Avatar != null && staffViewModel.Avatar.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                        // Tạo tên file độc nhất để tránh trùng lặp
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + staffViewModel.Avatar.FileName;
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await staffViewModel.Avatar.CopyToAsync(stream);
                        }
                        staff.Avatar = "img/" + uniqueFileName;
                    }
                    else
                    {

                        staff.Avatar = item;
                    }
                    staff.FullName = staffViewModel.FullName;
   
                     staff.Position = staffViewModel.Position;
                    staff.FbLink = staffViewModel.FbLink;
                     staff.InstagramLink = staffViewModel.InstagramLink;
                    staff.TwLink = staffViewModel.TwLink;
                    staff.Email = staffViewModel.Email;
                    staff.PhoneNumber=staffViewModel.PhoneNumber;
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staffViewModel.Id))
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
            return View(staffViewModel);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Staffs == null)
            {
                return Problem("Entity set 'BTLDAM.Staffs'  is null.");
            }
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
          return (_context.Staffs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
