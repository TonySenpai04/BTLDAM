﻿using System.ComponentModel.DataAnnotations;

namespace BaiTapLonDuAnMau.Models
{
    public class RoomViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập RoomNumber! ")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập RoomType! ")]
        public string RoomType { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập Price! ")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập số lượng Bed! ")]
        public int Bed { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập số lượng Bath! ")]
        public int Bath { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập Wifi(0 là không có Wifi,1 là có Wiff)! ")]
        public int Wifi { get; set; }
        public string? Description { get; set; }
        public double Stars { get; set; }
        public double? CountRate { get; set; }
        public string? Status { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public double Area { get; set; }
        public int FloorNumber { get; set; }
      
    }
}
