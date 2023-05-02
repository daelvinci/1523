namespace WebApplication1.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }

        public ICollection<BookTag> BookTags { get; set; }

    }
}
