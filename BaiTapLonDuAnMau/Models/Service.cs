using System.ComponentModel.DataAnnotations;

namespace BaiTapLonDuAnMau.Models
{
    public class Service
    {
        [Key] public int ID { get; set; }
        [Required] public string ServiceName { get; set; }
        public string IconName { get; set; }
        public string Description { get; set; }
        [Required] public decimal Price { get; set; }

    }
}
