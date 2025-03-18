using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public PlantsController(ApplicationDbContext context)
        {
            _context = context;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,PotSize,PlantHight,Amount,ImageName")] PlantModel plantModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plantModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plantModel);
        }

        // GET: Plants/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,PotSize,PlantHight,Amount,ImageName")] PlantModel plantModel)
        {
            if (id != plantModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
