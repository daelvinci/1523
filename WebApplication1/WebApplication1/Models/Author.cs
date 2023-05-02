using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Pkcs;

namespace WebApplication1.Models
{
    public class Author
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(35)]
        [Required]
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
