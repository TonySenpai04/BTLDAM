﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiTapLonDuAnMau.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace BaiTapLonDuAnMau.Controllers
{
    public class AccountController : BaseController
    {
        private readonly BTLDAM _context;

        public AccountController(BTLDAM context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Username,string Password)
        {
            Account account = new Account();
            
            if (ModelState.IsValid)
            {
                var userLogin = await _context.Accounts.FirstOrDefaultAsync(m => m.Username == Username);
                if (userLogin == null)
                {
                    ModelState.AddModelError("", "Thông tin tài khoản mật khẩu không chính xác!");
                    return View();

                }
                else
                {
                    account.Id = userLogin.Id;
                    account.Username = userLogin.Username;
                    account.Password = userLogin.Password;
                    account.StaffId=userLogin.StaffId;  
                    account.Status=userLogin.Status;
                    account.Type= userLogin.Type;
                    SHA256 hasMethod = SHA256.Create();
                    if (Util.Cryptography.VerifyHash(hasMethod, Password, userLogin.Password))
                    {
                        if (userLogin.Type == 0)
                        {
                            HttpContext.Session.SetString("IsAdmin", "true");

                        }
                        else if(userLogin.Type == 1)
                        {
                            HttpContext.Session.SetString("IsManager", "true");

                        }
                        else
                        {
                            HttpContext.Session.SetString("IsEmployee", "true");
                        }
                        CurrentUser = userLogin.Username;

                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {

                        ModelState.AddModelError("", "Thông tin tài khoản mật khẩu không chính xác!");
                        return View();
                    }
                }
            }
            return View(account);
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account account)
        {

            if (ModelState.IsValid)
            {

                SHA256 hasMethod = SHA256.Create();
                account.Password = Util.Cryptography.GetHash(hasMethod, account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));


            }
            return View(account);

        }
        public IActionResult LogOut()
        {
            CurrentUser = "";
            return RedirectToAction(nameof(Login));
        }

        // GET: Account
        public async Task<IActionResult> Index()
        {
            var bTLDAM = _context.Accounts.Include(a => a.Staff);
            return View(await bTLDAM.ToListAsync());
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Account/Create
        public IActionResult Create()
        {
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName");
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Status,Type,StaffId")] Account account)
        {
            if (ModelState.IsValid)
            {
                SHA256 hasMethod = SHA256.Create();
                account.Password = Util.Cryptography.GetHash(hasMethod, account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", account.StaffId);
            return View(account);
        }

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", account.StaffId);
            return View(account);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Status,Type,StaffId")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            ViewData["StaffId"] = new SelectList(_context.Staffs, "Id", "FullName", account.StaffId);
            return View(account);
        }

        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'BTLDAM.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
          return (_context.Accounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}