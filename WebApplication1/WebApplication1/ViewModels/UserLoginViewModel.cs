using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class UserLoginViewModel
    {
        [Required]
        [MaxLength(25)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        [MaxLength(25)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
