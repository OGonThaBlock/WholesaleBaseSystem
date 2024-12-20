using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.Models
{
    public class Supplier
    {
        [Key]
        public int Supplier_id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

    }
}
