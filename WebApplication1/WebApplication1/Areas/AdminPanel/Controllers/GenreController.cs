using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Authorize]

    [Area("AdminPanel")]
    public class GenreController : Controller
    {
        private readonly PustokDbContext _context;
        public GenreController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var genres = _context.Genres.Include(x => x.Books).Skip((page - 1) * 3).Take(3).ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_context.Genres.Any(x => x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "This Category is already use");
                //ModelState.AddModelError("","The word must be start with 'A' ");
                return View();
            }
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x => x.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Genre existGenre = _context.Genres.Find(genre.Id);

            if (existGenre == null)
            {
                return NotFound();
            }

            if (genre.Name != existGenre.Name && _context.Genres.Any(x => x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "This name is already in use");
                return View();
            }

            existGenre.Name = genre.Name;
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.Find(id);
            if (genre == null)
                return NotFound();

            return View(genre);

        }

        [HttpPost]
        public IActionResult Delete(Genre genre)
        {
            Genre existGenre = _context.Genres.Find(genre.Id);

            if (existGenre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(existGenre);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
