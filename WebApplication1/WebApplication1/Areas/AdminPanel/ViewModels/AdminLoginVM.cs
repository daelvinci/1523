using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.AdminPanel.ViewModels
{
    public class AdminLoginVM
    {
        [Required]
        [MinLength(8)]
        [MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(25)]
        public string Password{ get; set; }
    }
}
