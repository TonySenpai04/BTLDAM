using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTapLonDuAnMau.Models
{
    public class PaymentDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Payment))]
        public int PaymentId { get; set; }
        [ForeignKey(nameof(RoomService))]
        public int RoomServiceId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }

        public Payment? Payment { get; set; }
        public RoomService? RoomService { get; set; }

    }
}
