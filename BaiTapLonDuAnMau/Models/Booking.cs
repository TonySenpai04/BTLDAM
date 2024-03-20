using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BaiTapLonDuAnMau.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        [Required]
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut{ get; set; }

        [Required]
        public int NumAdults { get; set; }

        [Required]
        public int NumChildren { get; set; }

        public string? SpecialRequests { get; set; }

        public Room? Room { get; set; }
    }
}
