using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Authorize]

    [Area("AdminPanel")]
    public class TagController : Controller
    {
        private readonly PustokDbContext _context;
        public TagController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            var tags = _context.Tags.Include(x => x.BookTags).Skip((page - 1) * 3).Take(3).ToList();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]

        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_context.Tags.Any(x => x.TagName == tag.TagName))
            {
                ModelState.AddModelError("Name", "This Category is already use");
                //ModelState.AddModelError("","The word must be start with 'A' ");
                return View();
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Tag tag = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tag== null)
            {
                return NotFound();
            }
            return View(tag);
        }

        [HttpPost]
        public IActionResult Edit(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Tag existTag= _context.Tags.Find(tag.Id);

            if (existTag == null)
            {
                return NotFound();
            }

            if (tag.TagName != existTag.TagName && _context.Tags.Any(x => x.TagName == tag.TagName))
            {
                ModelState.AddModelError("Name", "This name is already in use");
                return View();
            }

            existTag.TagName = tag.TagName;
            _context.SaveChanges();
            return RedirectToAction("index");
        }


        public IActionResult Delete(int id)
        {
            Tag tag= _context.Tags.Find(id);
            if (tag == null)
                return NotFound();

            return View(tag);

        }

        [HttpPost]
        public IActionResult Delete(Tag tag)
        {
            Tag existTag = _context.Tags.Find(tag.Id);

            if (existTag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(existTag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


    }
}
