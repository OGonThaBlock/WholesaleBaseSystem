using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class DeliveryContent
    {
        [Key]
        public int Id { get; set; }
        public int Product_id { get; set; }
        public int DeliveryNumber { get; set; }
        public int Amount { get; set; }

    }
}
