using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class ForgotPasswordVM
    {
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }
}
