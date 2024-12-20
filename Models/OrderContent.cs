using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class OrderContent
    {
        [Key]
        public int Id { get; set; }
        public int Product_id { get; set; }
        public int OrderNumber { get; set; }
        public int Amount { get; set; }

    }
}
