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

        public string? FbLink { get; set; }
        public string? TwLink { get; set; }
        public string? InstagramLink { get; set; }
    }
}
