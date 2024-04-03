using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTapLonDuAnMau.Models;
using System.Globalization;

namespace BaiTapLonDuAnMau.Controllers
{
    public class RoomController : BaseController
    {
        private readonly BTLDAM _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomController(BTLDAM context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Room
        public async Task<IActionResult> Index()
        {
            if (IsLogin && string.Compare(ViewBag.IsLogin, "1", true) == 0)
            {
                return _context.Rooms != null ?
                    View(await _context.Rooms.ToListAsync()) :
                    Problem("Entity set 'BTLDAM.Rooms'  is null.");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeIndex()
        {
            if (IsLogin )
            {
                return _context.Rooms != null ?
                        View(await _context.Rooms.ToListAsync()) :
                        Problem("Entity set 'BTLDAM.Rooms'  is null.");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
           
        }
        public async Task<IActionResult> EmployeeEdit(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            IFormFile formFile = null;
            string formfileAString = "";
            string relativePath = room.ImageUrl;
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
            // Để truyền dữ liệu của tài khoản cần chỉnh sửa vào view, bạn cần truyền nó thông qua model hoặc ViewBag
            RoomViewModel viewModel = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Price = room.Price,
                Bath = room.Bath,
                Area = room.Area,
                Bed = room.Bed,
                Description = room.Description,
                Stars = room.Stars,
                CountRate = room.CountRate,
                FloorNumber = room.FloorNumber,
                Status = room.Status,
                Wifi = room.Wifi,
                ImageUrl = formFile


            };


            return View(viewModel);
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeEdit(int id, RoomViewModel roomViewModel)
        {
            if (id != roomViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var room = await _context.Rooms.FindAsync(id);

                    if (room == null)
                    {
                        return NotFound();
                    }

                    string item = room.ImageUrl;
                    var uniqueFileName = "";
                    var filePath = "";

                    // Kiểm tra và lưu ảnh đại diện mới nếu có
                    if (roomViewModel.ImageUrl != null && roomViewModel.ImageUrl.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                        // Tạo tên file độc nhất để tránh trùng lặp
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + roomViewModel.ImageUrl.FileName;
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await roomViewModel.ImageUrl.CopyToAsync(stream);
                        }
                        room.ImageUrl = "img/" + uniqueFileName;
                    }
                    else
                    {

                        room.ImageUrl = item;
                    }
                    room.Id = roomViewModel.Id;
                    room.RoomNumber = roomViewModel.RoomNumber;
                    room.RoomType = roomViewModel.RoomType;
                    room.Price = roomViewModel.Price;
                    room.Description = roomViewModel.Description;
                    room.Bath = roomViewModel.Bath;
                    room.Area = roomViewModel.Area;
                    room.Bed = roomViewModel.Bed;
                    room.Stars = roomViewModel.Stars;
                    room.CountRate = roomViewModel.CountRate;
                    room.FloorNumber = roomViewModel.FloorNumber;
                    room.Status = roomViewModel.Status;
                    room.Wifi = roomViewModel.Wifi;

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(roomViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EmployeeIndex));
            }
            return View(roomViewModel);
        }
        //[HttpGet]
        [Route("RoomList")]
        //public async Task<IActionResult> RoomList()
        //{
        //    List<Room> rooms = await _context.Rooms.Where(r => r.Status == "Trống").ToListAsync();
        //    return View(rooms);
        //}
        public async Task<IActionResult> RoomList(string checkInDate, string checkOutDate, int adultCount, int childCount)
        {
            IQueryable<Room> roomsQuery = _context.Rooms.Where(r => r.Status == "Trống");

            // Kiểm tra xem có thông tin tìm kiếm được gửi lên từ form hay không
            if (!string.IsNullOrEmpty(checkInDate) && !string.IsNullOrEmpty(checkOutDate) && adultCount > 0)
            {
                // Phân tích chuỗi ngày tháng thành đối tượng DateTime
                DateTime checkIn = DateTime.ParseExact(checkInDate, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);
                DateTime checkOut = DateTime.ParseExact(checkOutDate, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);
                if (childCount > 0)
                {
                    // Xử lý thông tin tìm kiếm và truy vấn cơ sở dữ liệu để lấy danh sách các phòng phù hợp
                    roomsQuery = roomsQuery.Where(r => r.Bed >= (adultCount + childCount) / 2);
                }
                else
                {
                    roomsQuery = roomsQuery.Where(r => r.Bed >= adultCount );
                }
            }

            // Lấy danh sách phòng từ truy vấn
            List<Room> rooms = await roomsQuery.ToListAsync();

            // Trả về view hiển thị danh sách phòng
            return View(rooms);
        }


        public async Task<IActionResult> RoomDetail(int Id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == Id);

            if (room == null)
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy phòng với Id cung cấp
            }

            return View(room);
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Room/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Room/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel roomViewModel)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = "";
                var filePath = "";
                var file = "";
                if (roomViewModel.ImageUrl != null && roomViewModel.ImageUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                    // Tạo tên file độc nhất để tránh trùng lặp
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + roomViewModel.ImageUrl.FileName;
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    file = "img/" + uniqueFileName;
                    // Lưu file vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await roomViewModel.ImageUrl.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn của file vào trường Avatar
                    //account.Avatar = "/images/" + uniqueFileName;



                }
                Room room = new Room
                {
                    Id = roomViewModel.Id,
                    RoomNumber = roomViewModel.RoomNumber,
                    RoomType = roomViewModel.RoomType,
                    Price = roomViewModel.Price,
                    Bath = roomViewModel.Bath,
                    Area = roomViewModel.Area,
                    Bed = roomViewModel.Bed,
                    Description = roomViewModel.Description,
                    Stars = roomViewModel.Stars,
                    CountRate = roomViewModel.CountRate,
                    FloorNumber = roomViewModel.FloorNumber,
                    Status = roomViewModel.Status,
                    Wifi = roomViewModel.Wifi,
                    ImageUrl = file



                };

                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roomViewModel);
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            IFormFile formFile = null;
            string formfileAString = "";
            string relativePath = room.ImageUrl;
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
            // Để truyền dữ liệu của tài khoản cần chỉnh sửa vào view, bạn cần truyền nó thông qua model hoặc ViewBag
            RoomViewModel viewModel = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Price = room.Price,
                Bath = room.Bath,
                Area = room.Area,
                Bed = room.Bed,
                Description = room.Description,
                Stars = room.Stars,
                CountRate = room.CountRate,
                FloorNumber = room.FloorNumber,
                Status = room.Status,
                Wifi = room.Wifi,
                ImageUrl = formFile


            };


            return View(viewModel);
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel roomViewModel)
        {
            if (id != roomViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var room = await _context.Rooms.FindAsync(id);

                    if (room == null)
                    {
                        return NotFound();
                    }

                    string item = room.ImageUrl;
                    var uniqueFileName = "";
                    var filePath = "";

                    // Kiểm tra và lưu ảnh đại diện mới nếu có
                    if (roomViewModel.ImageUrl != null && roomViewModel.ImageUrl.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                        // Tạo tên file độc nhất để tránh trùng lặp
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + roomViewModel.ImageUrl.FileName;
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await roomViewModel.ImageUrl.CopyToAsync(stream);
                        }
                        room.ImageUrl = "img/" + uniqueFileName;
                    }
                    else
                    {

                        room.ImageUrl = item;
                    }
                    room.Id = roomViewModel.Id;
                    room.RoomNumber = roomViewModel.RoomNumber;
                    room.RoomType = roomViewModel.RoomType;
                    room.Price = roomViewModel.Price;
                    room.Description = roomViewModel.Description;
                    room.Bath = roomViewModel.Bath;
                    room.Area = roomViewModel.Area;
                    room.Bed = roomViewModel.Bed;
                    room.Stars = roomViewModel.Stars;
                    room.CountRate = roomViewModel.CountRate;
                    room.FloorNumber = roomViewModel.FloorNumber;
                    room.Status = roomViewModel.Status;
                    room.Wifi = roomViewModel.Wifi;

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(roomViewModel.Id))
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
            return View(roomViewModel);
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rooms == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rooms == null)
            {
                return Problem("Entity set 'BTLDAM.Rooms'  is null.");
            }
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return (_context.Rooms?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

