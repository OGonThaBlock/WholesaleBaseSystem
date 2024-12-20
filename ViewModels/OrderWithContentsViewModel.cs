using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WholesaleBase.ViewModels
{
    public class OrderWithContentsViewModel
    {
        [Required]
        [Display(Name = "Customer")]
        public int Customer_id { get; set; } // ID выбранного клиента

        //public string CustomerName { get; set; } // Отображаемое имя клиента (для данных отображения)

        public DateTime Date { get; set; } = DateTime.Now;

        public string CurrentStatus { get; set; } = "Ожидает";

        public List<OrderContentViewModel> OrderContents { get; set; } = new List<OrderContentViewModel>();
    }

    public class OrderContentViewModel
    {
        public int Product_id { get; set; }
        public int Amount { get; set; }
    }
}