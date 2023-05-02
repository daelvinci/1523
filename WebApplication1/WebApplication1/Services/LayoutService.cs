using WebApplication1.DAL;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Packaging.Signing;

using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Web;
using System.Security.Claims;

namespace WebApplication1.Services
{
    public class LayoutService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private PustokDbContext _context { get; set; }
        public LayoutService(PustokDbContext context, IHttpContextAccessor accessor, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _accessor = accessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }

        public BasketViewModel GetBasket()
        {
            BasketViewModel basketVM = new BasketViewModel();
            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var basketItems = _context.BasketItems.Where(x=>x.AppUserId==userId).Include(x=>x.Book).ToList();
                
                if (userId != null && _accessor.HttpContext.User.IsInRole("Member"))
                {
                    foreach (var item in basketItems)
                    {

                        basketVM.BasketItems.Add(new BasketItemViewModel
                        {
                            Book = item.Book,
                            Count = item.Count

                        });
                        var price = item.Book.DiscountPercent > 0 ? (item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100) : item.Book.SalePrice;
                        basketVM.TotalPrice += (price * item.Count);


                    }
                }

            }
            else
            {

                List<BasketCookieItemViewModel> basketItems = new List<BasketCookieItemViewModel>();
                var basket = _accessor.HttpContext.Request.Cookies["basket"];
                if (basket != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);

                }
                foreach (var item in basketItems)
                {

                    var book = _context.Books.Include(x => x.BookImages.Where(bi => bi.PosterStatus == true))?.FirstOrDefault(x => x.Id == item.BookId);
                    basketVM.BasketItems.Add(new BasketItemViewModel
                    {
                        Book = book,
                        Count = item.Count

                    });
                    var price = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice;
                    basketVM.TotalPrice += (price * item.Count);


                }
            }
            return basketVM;
        }

    }
}
