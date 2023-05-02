using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Attributes.ValidationAttributes;

namespace WebApplication1.Models
{
    public class Book
    {
        public int Id { get; set; }
        [MaxLength(30,ErrorMessage ="az yazda xiyar")]
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool IsNew { get; set; }
        public bool IsBestSeller { get; set; }  
        public bool IsDeleted { get; set; }
        public bool StockStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public Author Author{ get; set; }
        public List<BookTag> BookTags{ get; set; }
        public List<BookImage> BookImages { get; set; } = new List<BookImage>();

        [NotMapped]
        [FileMaxLength(2097152)]
        [AllowedFileType("image/png", "image/jpeg", "image/jpg")]

        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();

        [NotMapped]
        [FileMaxLength(2097152)]
        [AllowedFileType("image/png","image/jpeg","image/jpg")]
        public IFormFile PosterFile { get; set; }

        [NotMapped]
        [FileMaxLength(2097152)]
        [AllowedFileType("image/png", "image/jpeg")]

        public IFormFile HoverPosterFile { get; set; }

        [NotMapped]
        public List<int> TagIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> BookImageIds { get; set; } = new List<int>();
        public List<BookReview> BookReviews { get; set; }

    }
}
