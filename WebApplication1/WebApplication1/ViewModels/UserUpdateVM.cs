using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class UserUpdateVM
    {
        [Required]
        [MaxLength(25)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MaxLength(25)]
        public string FullName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        [MaxLength(25)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        [MaxLength(25)]

        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        [MaxLength(25)]
        [Compare(nameof(Password))]

        public string ConfirmPassword { get; set; } 
    }
}
