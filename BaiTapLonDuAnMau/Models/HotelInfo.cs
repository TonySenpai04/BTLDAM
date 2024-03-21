using System.ComponentModel.DataAnnotations;

namespace BaiTapLonDuAnMau.Models
{
    public class HotelInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string HotelName { get; set; }
        [Required]
        public string PhoneNumber { get; set;}
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        public string? FbLink { get; set; }
        public string? YtLink { get; set; }
        public string? TwLink { get; set; }
        public string? InLink { get; set; }
       
    }
}
