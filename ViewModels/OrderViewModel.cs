using WholesaleBase.Models;

namespace WholesaleBase.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderContentViewModelPrice> OrderContents { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostWithDiscount { get; set; } // Вычисляется на основе PromoCode
    }

    public class OrderContentViewModelPrice
    {
        public int Id { get; set; }
        public int Product_id { get; set; }
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice => Amount * ProductPrice;
    }
}
