using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Packaging.Signing;
using System.Security.Claims;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class BookController : Controller
    {
        private PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public BookController(PustokDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var book = _context.Books
                .Include(x => x.BookReviews).ThenInclude(u => u.AppUser)
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.BookTags).ThenInclude(bt => bt.Tag)
                .Include(x => x.BookImages)
                .FirstOrDefault(x => x.Id == id);
            BookDetailVM bookDetailVM = new()
            {
                Book = book,
                RelatedBooks = _context.Books
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .Where(x => x.GenreId == book.GenreId && x.Id != book.Id).Take(6).ToList(),
                BookReview = new BookReview { BookId = book.Id },
                AvgRate = book.BookReviews.Any() ? (int)Math.Ceiling(book.BookReviews.Average(x => x.Rate)) : 0,
                IsAllowed = book.BookReviews.Any(x => x.AppUserId == userId) ? false : true




            };

            return View(bookDetailVM);

        }

        [HttpPost]
        public async Task<IActionResult> Review(BookReview review)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("login", "account", new { Url = Url.Action("detail", "book", new { id = review.BookId }) });

            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null || user.IsAdmin)
            {
                return RedirectToAction("login", "account", new { Url = Url.Action("detail", "book", new { id = review.BookId }) });
            }

            var book = _context.Books
                .Include(x => x.BookReviews).ThenInclude(u => u.AppUser)
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.BookTags).ThenInclude(bt => bt.Tag)
                .Include(x => x.BookImages)
                .FirstOrDefault(x => x.Id == review.BookId);
            if (book == null)
            {
                return NotFound();
            }
            BookDetailVM bookDetailVM = new()
            {
                Book = book,
                RelatedBooks = _context.Books
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .Where(x => x.GenreId == book.GenreId && x.Id != book.Id).Take(6).ToList(),
                AvgRate = book.BookReviews.Any() ? (int)Math.Ceiling(book.BookReviews.Average(x => x.Rate)) : 0,
                BookReview = new BookReview()

            };

            if (!ModelState.IsValid)
            {
                bookDetailVM.BookReview = review;
            }

            review.AppUserId = user.Id;
            review.PostedAt = DateTime.UtcNow;
            _context.BookReviews.Add(review);
            _context.SaveChanges();
            return View("detail", bookDetailVM);
        }
        public IActionResult GetBookModal(int id)
        {
            var book = _context.Books

                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.BookImages)
                .FirstOrDefault(x => x.Id == id);

            return PartialView("BookModalPartial", book);
        }

        public async Task<IActionResult> AddToBasket(int id)
        {
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = _userManager.Users.Include(x => x.BasketItems).FirstOrDefault(x => x.UserName == User.Identity.Name && !x.IsAdmin);
            }
            

            if (_context.Books.Find(id) == null)
            {
                return NotFound();
            }

            if (user == null)
            {
                var basket = HttpContext.Request.Cookies["basket"];
                List<BasketCookieItemViewModel> basketItems;

                if (basket == null)
                    basketItems = new List<BasketCookieItemViewModel>();
                else
                    basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);

                var wantedBook = basketItems.FirstOrDefault(x => x.BookId == id);
                if (wantedBook != null)
                    wantedBook.Count++;
                else
                    basketItems.Add(new BasketCookieItemViewModel { BookId = id, Count = 1 });

                BasketViewModel basketVM = new BasketViewModel();

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

                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItems));
                return PartialView("BookCartPartial", basketVM);
            }
            else
            {
                var basketItems = user.BasketItems.Where(x => x.AppUserId == user.Id).ToList();
                var basketItem = user.BasketItems.FirstOrDefault(x => x.BookId == id);
                if (basketItem != null)
                {
                    basketItem.Count++;
                }
                else
                {
                    user.BasketItems.Add(new BasketItem { BookId = id, AppUserId = user.Id, Count = 1 });
                }


                _context.SaveChanges();

                BasketViewModel basketVM = new BasketViewModel();

                foreach (var item in user.BasketItems)
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

                return PartialView("BookCartPartial", basketVM);
            }
        }

        public IActionResult ShowBasket()
        {
            var basket = HttpContext.Request.Cookies["basket"];
            var basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);
            return Json(basketItems);
        }

        public IActionResult DeleteFromBasket(int id)
        {
            BasketViewModel basketVM = new BasketViewModel();

            if (!User.Identity.IsAuthenticated)
            {
                if (_context.Books.Find(id) == null)
                {
                    return NotFound();
                }

                var basket = HttpContext.Request.Cookies["basket"];
                List<BasketCookieItemViewModel> basketItems;

                if (basket == null)
                    basketItems = new List<BasketCookieItemViewModel>();
                else
                    basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);


                var toDelete = basketItems.FirstOrDefault(x => x.BookId == id);
                basketItems.Remove(toDelete);


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
                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItems));
            }
            else
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null && HttpContext.User.IsInRole("Member"))
                {

                    var item = _context.BasketItems.FirstOrDefault(x => x.BookId == id && x.AppUserId == userId);

                    if (item == null) return NotFound();

                    _context.BasketItems.Remove(item);
                    _context.SaveChanges();

                    var basketItems = _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == userId).ToList();

                    foreach (var bi in basketItems)
                    {


                        var price = bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice;
                        basketVM.TotalPrice += (price * bi.Count);


                    }
                }
            }


            return PartialView("BookCartPartial", basketVM);
        }
    }
}
