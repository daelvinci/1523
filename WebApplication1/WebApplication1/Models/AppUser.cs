using Microsoft.AspNetCore.Identity;
using WebApplication1.ViewModels;

namespace WebApplication1.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsAdmin { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
