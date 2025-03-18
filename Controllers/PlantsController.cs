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
    public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string wwwRootPath;

        public PlantsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath;
        }

        // GET: Plants
        public async Task<IActionResult> Index()
        {
            return View(await _context.Plants.ToListAsync());
        }

        // GET: Plants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantModel = await _context.Plants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantModel == null)
            {
                return NotFound();
            }

            return View(plantModel);
        }

        // GET: Plants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plants/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,PotSize,PlantHight,Amount,ImageFile")] PlantModel plantModel)
        {
            if (ModelState.IsValid)
            {
                // check for image
                if (plantModel.ImageFile != null)
                {
                    // Generate a unique filename
                    string fileName = Path.GetFileNameWithoutExtension(plantModel.ImageFile.FileName);
                    string extension = Path.GetExtension(plantModel.ImageFile.FileName);

                    plantModel.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    // Save the image to the wwwroot/images/news folder
                    string path = Path.Combine(wwwRootPath + "/images", fileName);

                    // Store in file system
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await plantModel.ImageFile.CopyToAsync(fileStream);
                    }

                }
                else
                {
                    plantModel.ImageName = "default.jpg";
                }
                _context.Add(plantModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plantModel);
        }

        // GET: Plants/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantModel = await _context.Plants.FindAsync(id);
            if (plantModel == null)
            {
                return NotFound();
            }
            return View(plantModel);
        }

        // POST: Plants/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,PotSize,PlantHight,Amount,ImageFile")] PlantModel plantModel)
        {
            if (id != plantModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing plant item to get the image name
                    var existingPlant = await _context.Plants.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Keep the existing image if no new image is uploaded
                    if (plantModel.ImageFile == null)
                    {
                        plantModel.ImageName = existingPlant?.ImageName;
                    }
                    else
                    {
                        // If a new image is uploaded, delete the old image if it's not the default image
                        if (existingPlant?.ImageName != "default.jpg" && existingPlant?.ImageName != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath + "/images", existingPlant.ImageName);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Generate a unique filename
                        string fileName = Path.GetFileNameWithoutExtension(plantModel.ImageFile.FileName);
                        string extension = Path.GetExtension(plantModel.ImageFile.FileName);

                        plantModel.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                        // Save in wwwroot/images
                        string path = Path.Combine(wwwRootPath + "/images", fileName);

                        // Save in file system
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await plantModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(plantModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlantModelExists(plantModel.Id))
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
            return View(plantModel);
        }

        // GET: Plants/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantModel = await _context.Plants
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantModel == null)
            {
                return NotFound();
            }

            return View(plantModel);
        }

        // POST: Plants/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plantModel = await _context.Plants.FindAsync(id);
            if (plantModel != null)
            {
                _context.Plants.Remove(plantModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlantModelExists(int id)
        {
            return _context.Plants.Any(e => e.Id == id);
        }
    }
}
