using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class Order
    {
        [Key]
        public int Number { get; set; }
        public int Customer_id { get; set; }
        public DateTime Date { get; set; }
        public string CurrentStatus { get; set; }

    }
}
