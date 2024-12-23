using WholesaleBase.Models;

namespace WholesaleBase.ViewModels
{
    public class DeliveryViewModel
    {
        public Delivery Delivery { get; set; }
        public List<DeliveryContentViewModelPrice> DeliveryContents2 { get; set; }
    }

    public class DeliveryContentViewModelPrice
    {
        public int Id { get; set; }
        public int Product_id { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
    }
}
