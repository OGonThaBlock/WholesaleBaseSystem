using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class Customer
    {
        [Key]
        public int Customer_id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        
        public float PromoCode { get; set; }
        // Дата, с которой началась текущая скидка
        public DateTime PromoCodeDate { get; set; } = DateTime.MinValue;
    }
}
