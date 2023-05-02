using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class BookDetailVM
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public BookReview BookReview { get; set; }
        public int AvgRate { get; set; }
        public bool IsAllowed { get; set; }
    }
}
