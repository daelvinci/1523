using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private PustokDbContext _context {get;set;}
        public HomeController(PustokDbContext context)
        {
            _context = context;
        }
      
        public IActionResult Index()
        {
            ViewModel vm = new ViewModel
            { 
                Sliders = _context.Sliders.ToList(),
                Features= _context.Features.ToList(),
                BestSellerBooks= _context.Books.Include(x=>x.Author).Include(x=>x.BookImages).Where(x=>x.IsBestSeller).Take(25).ToList(),
                NewBooks= _context.Books.Include(x=>x.Author).Include(x=>x.BookImages).Where(x=>x.IsNew).Take(25).ToList(),
                DiscountedBooks= _context.Books.Include(x=>x.Author).Include(x=>x.BookImages).Where(x=>x.DiscountPercent>0).ToList()
            };
            return View(vm);
        }

        public IActionResult Contact()
        {
            return View();
        }

        //public IActionResult SetSession(string key, string value)
        //{
        //    HttpContext.Session.SetString(key, value);
        //    return RedirectToAction("Index");
        //}

        //public IActionResult GetSession(string key)
        //{
        //    var value = HttpContext.Session.GetString(key);
        //    return Content(value);
        //}

        public IActionResult SetCookies (string key, string value)
        {
            HttpContext.Response.Cookies.Append(key, value);
            return RedirectToAction("Index");
        }

        public IActionResult GetCookies(string key)
        {
            var value = HttpContext.Request.Cookies[key];
            return Content(value);
        }

        
    }
}