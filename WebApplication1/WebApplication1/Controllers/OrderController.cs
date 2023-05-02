using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private readonly PustokDbContext _context;

        public OrderController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Checkout()
        {
            CheckoutVM checkout = new()
            {
                BasketItems = GetBasketItems(),
                Order = new Order()

            };
            checkout.TotalPrice = checkout.BasketItems.Sum(x => x.Count * x.Price);
            return View(checkout);
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (!ModelState.IsValid) return View();


            var basket = HttpContext.Request.Cookies["basket"];

            if (basket == null) return NotFound();

            List<BasketCookieItemViewModel> basketItems;
            basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);

            foreach (var item in basketItems)
            {
                var book = _context.Books.Find(item.BookId);

                if (book == null) return NotFound();

                order.OrderItems.Add(new OrderItem { BookId = item.BookId, Count = item.Count, DiscountPercent = book.DiscountPercent, SalePrice = book.SalePrice, CostPrice = book.CostPrice });
            }

            order.CreatedAt = DateTime.UtcNow;
            order.Status = Enums.OrderStatus.pending;
            _context.Orders.Add(order);
            _context.SaveChanges();

            HttpContext.Response.Cookies.Delete("basket");

            return RedirectToAction("index", "home");
        }

        private List<CheckoutBookItemViewModel> GetBasketItems()
        {
            var basket = HttpContext.Request.Cookies["basket"];
            List<BasketCookieItemViewModel> basketCookieItems;
            List<CheckoutBookItemViewModel> basketItems = new();

            if (basket == null)
                basketCookieItems = new List<BasketCookieItemViewModel>();
            else
                basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basket);



            foreach (var item in basketCookieItems)
            {
                Book book = _context.Books.Find(item.BookId);
                CheckoutBookItemViewModel cbi = new()
                {
                    Name = book.Name,
                    Count = item.Count,
                    Price = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                };

                basketItems.Add(cbi);
            }
            return basketItems;


        }
    }
}
