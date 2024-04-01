using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTapLonDuAnMau.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
       
        public decimal? TotalAmount { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }
        [Required]
        public decimal Deposit { get; set; }

        public Room? Room { get; set; }

    }
}
