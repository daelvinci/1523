using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class ResetPasswordVM
    {
        [MaxLength(25)]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [MaxLength(25)]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        [Compare (nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
