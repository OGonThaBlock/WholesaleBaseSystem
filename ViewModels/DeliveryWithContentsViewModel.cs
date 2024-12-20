using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.ViewModels
{
    public class DeliveryWithContentsViewModel
    {
        public int Supplier_id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string CurrentStatus { get; set; }

        public List<DeliveryContentViewModel> DeliveryContents { get; set; } = new List<DeliveryContentViewModel>();
    }

    public class DeliveryContentViewModel
    {
        public int Product_id { get; set; }
        public int Amount { get; set; }
    }
}
