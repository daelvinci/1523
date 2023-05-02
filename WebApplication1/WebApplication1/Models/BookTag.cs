using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BookTag
    { 
        public int BookId { get; set; }
        public int TagId { get; set; }
        public Book Book { get; set; }
        public Tag Tag { get; set; }

    }
}
