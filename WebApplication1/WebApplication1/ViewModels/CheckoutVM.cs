using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class CheckoutVM
    {
        public List<CheckoutBookItemViewModel> BasketItems { get; set; }
        public Order Order { get; set; } = new Order();
        public decimal TotalPrice { get; set; }
    }
}
