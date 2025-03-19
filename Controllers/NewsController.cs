using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plantopia.Data;
using Plantopia.Models;

namespace Plantopia.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string wwwRootPath;

        public NewsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var news = await _context.News.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return View(news);
        }


        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsModel = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsModel == null)
            {
                return NotFound();
            }

            return View(newsModel);
        }

        // GET: News/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,ImageFile")] NewsModel newsModel)
        {
            if (ModelState.IsValid)
            {

                // check for image
                if (newsModel.ImageFile != null)
                {
                    // Generate a unique filename
                    string fileName = Path.GetFileNameWithoutExtension(newsModel.ImageFile.FileName);
                    string extension = Path.GetExtension(newsModel.ImageFile.FileName);

                    newsModel.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    // Save the image to the wwwroot/images/news folder
                    string path = Path.Combine(wwwRootPath + "/images", fileName);

                    // Store in file system
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await newsModel.ImageFile.CopyToAsync(fileStream);
                    }

                }
                else
                {
                    newsModel.ImageName = "default.jpg";
                }
                _context.Add(newsModel);

                // Add the current user as the creator of the news
                newsModel.CreatedBy = User.Identity?.Name ?? "Unknown";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsModel);
        }

        // GET: News/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsModel = await _context.News.FindAsync(id);
            if (newsModel == null)
            {
                return NotFound();
            }
            return View(newsModel);
        }

        // POST: News/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,ImageName,ImageFile")] NewsModel newsModel)
        {
            if (id != newsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing news item to get the image name 
                    var existingNews = await _context.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);

                    // Keep the existing image if no new image is uploaded
                    if (newsModel.ImageFile == null)
                    {
                        newsModel.ImageName = existingNews?.ImageName;
                    }
                    else
                    {
                        // If a new image is uploaded, delete the old image if it's not the default image
                        if (existingNews?.ImageName != "default.jpg" && existingNews?.ImageName != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath + "/images", existingNews.ImageName);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Generate a unique filename
                        string fileName = Path.GetFileNameWithoutExtension(newsModel.ImageFile.FileName);
                        string extension = Path.GetExtension(newsModel.ImageFile.FileName);

                        newsModel.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                        // Save in wwwroot/images
                        string path = Path.Combine(wwwRootPath + "/images", fileName);

                        // Save in file system
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await newsModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(newsModel);

                    // Add the current user as the editor of the news
                    newsModel.CreatedBy = User.Identity?.Name ?? "Unknown";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsModelExists(newsModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(newsModel);
        }

        // GET: News/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsModel = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (newsModel == null)
            {
                return NotFound();
            }

            return View(newsModel);
        }

        // POST: News/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsModel = await _context.News.FindAsync(id);
            if (newsModel != null)
            {
                _context.News.Remove(newsModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsModelExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
