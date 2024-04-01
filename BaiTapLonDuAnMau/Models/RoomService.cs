using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTapLonDuAnMau.Models
{
    public class RoomService
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Booking))]
        public int? BookingId { get; set; }
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }

        public Booking? Booking { get; set; }
        public Room? Room { get; set; }

        public Service? Service { get; set; }
    }
}
