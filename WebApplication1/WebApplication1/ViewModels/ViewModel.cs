using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class ViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; }
        public List<Book> BestSellerBooks { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> DiscountedBooks { get; set; }
        public List<Book> Books { get; set; }



    }
}
