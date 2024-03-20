using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTapLonDuAnMau.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int Type { get; set; }

        [Required]
        [ForeignKey(nameof(Staff))]
        public int StaffId { get; set; }

        public Staff? Staff { get; set; }
    }
}
