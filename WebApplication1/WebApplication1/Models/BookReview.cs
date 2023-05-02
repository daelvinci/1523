using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BookReview
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int BookId { get; set; }
        public string Text { get; set; }
        [Range(1,5)]
        public byte Rate { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public Book Book { get; set; }
        public AppUser AppUser { get; set; }
    }
}
