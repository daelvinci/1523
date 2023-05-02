using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication1.DAL;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Areas.AdminPanel.Controllers
{
    [Authorize]

    [Area("AdminPanel")]
    public class SliderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            var data = _context.Sliders.OrderBy(x => x.Order).Skip((page - 1) * 3).Take(3).ToList();
            return View(data);
        }
        public IActionResult Create()
        {
            var order = _context.Sliders.Max(x => x.Order);
            ViewBag.Order = order + 1;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (slider.File.FileName.Length > 100)
            {
                ModelState.AddModelError("File", "File Name must be less than 100 char");
            }
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            if (slider.File.ContentType != "image/jpeg" && slider.File.ContentType != "image/png")
            {
                ModelState.AddModelError("File", "File format must be png,jpeg");
            }

            if (slider.File.Length > 2097152)
            {
                ModelState.AddModelError("File", "File size must be less than 2 mb");
            }



            slider.Img = FileManager.Save(slider.File, _env.WebRootPath + "/uploads/sliders");

            _context.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var data = _context.Sliders.Find(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var existSlider = _context.Sliders.Find(slider.Id);

            if (existSlider == null)
            {
                return NotFound();
            }


            if (slider.File.FileName.Length > 100)
            {
                ModelState.AddModelError("File", "File Name must be less than 100 char");
            }
            if (slider.File.ContentType != "image/jpeg" && slider.File.ContentType != "image/png")
            {
                ModelState.AddModelError("File", "File format must be png,jpeg");
            }

            if (slider.File.Length > 2097152)
            {
                ModelState.AddModelError("File", "File size must be less than 2 mb");
            }
            if (slider.File!=null )
            { 
                FileInfo file = new FileInfo(_env.WebRootPath + $"/uploads/sliders/{existSlider.Img}");
                file.Delete();
            }
            existSlider.Img = FileManager.Save(slider.File, _env.WebRootPath + "/uploads/sliders");
            existSlider.Order = slider.Order;
            existSlider.Title1 = slider.Title1;
            existSlider.Title2 = slider.Title2;
            existSlider.Desc = slider.Desc;
            existSlider.BtnUrl = slider.BtnUrl;
            existSlider.BtnTxt = slider.BtnTxt;

            _context.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider == null)
                return NotFound();

            return View(slider);

        }

        [HttpPost]
        public IActionResult Delete(Slider slider)
        {
            Slider existSlider = _context.Sliders.Find(slider.Id);

            if (existSlider == null)
            {
                return NotFound();
            }

            FileInfo file = new FileInfo(_env.WebRootPath + $"/uploads/sliders/{slider.File.FileName}");
            file.Delete();

            _context.Sliders.Remove(existSlider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
