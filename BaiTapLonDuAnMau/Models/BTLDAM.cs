using BaiTapLonDuAnMau.Models;
using Microsoft.EntityFrameworkCore;

namespace BaiTapLonDuAnMau.Models
{
    public class BTLDAM:DbContext
    {
        public BTLDAM(DbContextOptions<BTLDAM> options) : base(options) { }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Account> Accounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().ToTable("room");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<Service>().ToTable("Service");
            modelBuilder.Entity<Account>().ToTable("Account");
            // base.OnModelCreating(modelBuilder);
        }
        public DbSet<BaiTapLonDuAnMau.Models.Room> Room { get; set; } = default!;
        public DbSet<BaiTapLonDuAnMau.Models.Booking> Booking { get; set; } = default!;
        public DbSet<BaiTapLonDuAnMau.Models.Staff> Staff { get; set; } = default!;
        public DbSet<BaiTapLonDuAnMau.Models.Service> Service { get; set; } = default!;
        public DbSet<BaiTapLonDuAnMau.Models.Account> Account { get; set; } = default!;

    }
}
