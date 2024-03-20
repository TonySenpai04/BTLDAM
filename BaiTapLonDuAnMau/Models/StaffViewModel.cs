using System.ComponentModel.DataAnnotations;

namespace BaiTapLonDuAnMau.Models
{
    public class StaffViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public IFormFile? Avatar { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        [StringLength(12)]
        public string PhoneNumber { get; set; }
        public string? FbLink { get; set; }
        public string? TwLink { get; set; }
        public string? InstagramLink { get; set; }
        public string Email { get; set; }
    }
}
