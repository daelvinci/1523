using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Authorize]

    [Area("AdminPanel")]
    public class AuthorController : Controller
    {
        private readonly PustokDbContext _context;

        public AuthorController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            var authors = _context.Authors.Include(x=>x.Books).Skip((page-1)*3).Take(3).ToList();
            return View(authors);
        }
       

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
   
        public IActionResult Create(Author author)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if(_context.Authors.Any(x=>x.Name==author.Name))
            {
                ModelState.AddModelError("Name","This Name is Already Taken");
                //ModelState.AddModelError("","The word must be start with 'A' ");
                return View();
            }
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction("index");
        }


        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x=>x.Id==id );
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {
           if(!ModelState.IsValid)
            {
                return View();
            }

            Author existAuthor = _context.Authors.Find(author.Id);

            if(existAuthor==null)
            {
                return NotFound();
            }

            if (author.Name!=existAuthor.Name &&_context.Authors.Any(x => x.Name == author.Name))
            {
                ModelState.AddModelError("Name", "This name is already taken");
                return View();
            }

            existAuthor.Name = author.Name;
            existAuthor.BirthYear = author.BirthYear;

            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.Find(id);
            if (author == null)
                return NotFound();

            return View(author);

        }

        [HttpPost]
        public IActionResult Delete(Author author)
        {
            Author existAuthor = _context.Authors.Find(author.Id);

            if(existAuthor==null)
            {
                return NotFound();
            }

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();

            return RedirectToAction("index");   
        }
    }
}
