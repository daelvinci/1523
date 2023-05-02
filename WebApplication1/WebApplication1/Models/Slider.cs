using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Img { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Desc{ get; set; }
        public string BtnTxt { get; set; }
        public string BtnUrl { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
