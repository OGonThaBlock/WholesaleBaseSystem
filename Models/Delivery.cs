using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class Delivery
    {
        [Key]
        public int Number { get; set; }
        public int Supplier_id { get; set; }
        public DateTime Date { get; set; }
        public string CurrentStatus { get; set; }

    }
}