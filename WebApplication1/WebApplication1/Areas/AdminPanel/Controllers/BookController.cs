using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Drawing;
using WebApplication1.DAL;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Authorize]
    [Area("adminpanel")]
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1, int size=4)
        {
            var query = _context.Books
                .Include(x => x.BookImages)
            .Include(x => x.Genre)
            .Where(x => !x.IsDeleted);


            return View(PaginatedList<Book>.Create(query, page, size));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            CheckCreate(book);

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Tags = _context.Tags.ToList();
                ViewBag.Genres = _context.Genres.ToList();

                return View();
            }
            book.BookTags = book.TagIds.Select(x => new BookTag { TagId = x }).ToList();

            AddBook(book);

            book.CreatedAt = DateTime.UtcNow;

            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Book book = _context.Books
              .Include(x => x.BookTags)
              .Include(x => x.BookImages)
              .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

            if (book == null)
                return View("Error");

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            Book existBook = _context.Books
                .Include(x => x.BookImages)
                .Include(x => x.BookTags).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null)
                return View("Error");

            CheckEdit(book, existBook);

            existBook.SalePrice = book.SalePrice;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.CostPrice = book.CostPrice;
            existBook.Name = book.Name;
            existBook.GenreId = book.GenreId;
            existBook.AuthorId = book.AuthorId;
            existBook.Desc = book.Desc;
            existBook.IsNew = book.IsNew;
            existBook.StockStatus = book.StockStatus;
            existBook.IsBestSeller = book.IsBestSeller;


            var newBookTags = book.TagIds
                .FindAll(x => !existBook.BookTags.Any(bt => bt.TagId == x))
                .Select(x => new BookTag { TagId = x }).ToList();

            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));
            existBook.BookTags.AddRange(newBookTags);

            UpdateBookImages(book, existBook);


            existBook.ModifiedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("index");

        }


        public IActionResult Delete(int id)
        {

            Book book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();

            book.IsDeleted = true;
            _context.SaveChanges();

            return View(book);
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            Book existBook = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x=>x.Id==book.Id);

            if (existBook == null)
            {
                return NotFound();
            }

            DeleteBookImages(existBook);

            _context.Books.Remove(existBook);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


        private void CheckCreate(Book book)
        {
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found");
                return;
            }
            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Auhtor not found");
                return;
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found");
                return;
            }

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "Tag not found");
                    break;
                }
            }

            if (book.PosterFile == null)
                ModelState.AddModelError("PosterFile", "PosterFile is required");

            if (book.HoverPosterFile == null)
                ModelState.AddModelError("HoverPosterFile", "HoverPosterFile is required");
        }
        private void AddBook(Book book)
        {


            BookImage posterBi = new BookImage
            {
                Image = FileManager.Save(book.PosterFile, _env.WebRootPath + "/uploads/books"),
                PosterStatus = true,
            };

            BookImage hoverPosterBi = new BookImage
            {
                Image = FileManager.Save(book.HoverPosterFile, _env.WebRootPath + "/uploads/books"),
                PosterStatus = false,
            };

            book.BookImages.Add(posterBi);
            book.BookImages.Add(hoverPosterBi);

            foreach (var biFile in book.ImageFiles)
            {
                BookImage bi = new BookImage
                {
                    Image = FileManager.Save(biFile, _env.WebRootPath + "/uploads/books"),
                    PosterStatus = null,
                };

                book.BookImages.Add(bi);
            }
        }
        private void UpdateBookImages(Book book, Book existBook)
        {
            if (book.PosterFile != null)
            {
                var poster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                string oldImageName = poster.Image;

                poster.Image = FileManager.Save(book.PosterFile, _env.WebRootPath + "/uploads/books");
                FileManager.Delete(_env.WebRootPath + "/uploads/books", oldImageName);
            }

            if (book.HoverPosterFile != null)
            {
                var hoverPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                string oldImageName = hoverPoster.Image;

                hoverPoster.Image = FileManager.Save(book.HoverPosterFile, _env.WebRootPath + "/uploads/books");
                FileManager.Delete(_env.WebRootPath + "/uploads/books", oldImageName);
            }


            var removedFiles = existBook.BookImages.FindAll(x => x.PosterStatus == null && !book.BookImageIds.Contains(x.Id));

            FileManager.DeleteAll(_env.WebRootPath + "/uploads/books", removedFiles.Select(x => x.Image).ToList());

            existBook.BookImages.RemoveAll(x => removedFiles.Contains(x));


            foreach (var biFile in book.ImageFiles)
            {
                BookImage bi = new BookImage
                {
                    Image = FileManager.Save(biFile, _env.WebRootPath + "/uploads/books"),
                    PosterStatus = null,
                };

                existBook.BookImages.Add(bi);
            }
        }
        private void DeleteBookImages(Book existbook)
        {
            var removedFiles = existbook.BookImages;

            FileManager.DeleteAll(_env.WebRootPath + "/uploads/Books", existbook.BookImages.Select(x => x.Image).ToList());

            existbook.BookImages.RemoveAll(x => removedFiles.Contains(x));
        }
        private void CheckEdit(Book book, Book existBook)
        {
            if (existBook.AuthorId != book.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
                ModelState.AddModelError("AuthorId", "Author not found");

            if (existBook.GenreId != book.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
                ModelState.AddModelError("GenreId", "Genre not found");

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "Tag not found");
                    break;
                }
            }
        }


    }
}
