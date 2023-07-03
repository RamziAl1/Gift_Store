using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gifts_Store.Models;

namespace Gifts_Store.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly ModelContext _context;

        public AboutUsController(ModelContext context)
        {
            _context = context;
        }

        // GET: AboutUs
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.AboutUs.Include(a => a.Admin);
            return View(await modelContext.ToListAsync());
        }

        // GET: AboutUs/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.AboutUs == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs
                .Include(a => a.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutU == null)
            {
                return NotFound();
            }

            return View(aboutU);
        }

        // GET: AboutUs/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id");
            return View();
        }

        // POST: AboutUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IntroText,AdminId")] AboutU aboutU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aboutU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", aboutU.AdminId);
            return View(aboutU);
        }

        // GET: AboutUs/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.AboutUs == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs.FindAsync(id);
            if (aboutU == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", aboutU.AdminId);
            return View(aboutU);
        }

        // POST: AboutUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,IntroText,AdminId")] AboutU aboutU)
        {
            if (id != aboutU.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aboutU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutUExists(aboutU.Id))
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
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", aboutU.AdminId);
            return View(aboutU);
        }

        // GET: AboutUs/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.AboutUs == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs
                .Include(a => a.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutU == null)
            {
                return NotFound();
            }

            return View(aboutU);
        }

        // POST: AboutUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.AboutUs == null)
            {
                return Problem("Entity set 'ModelContext.AboutUs'  is null.");
            }
            var aboutU = await _context.AboutUs.FindAsync(id);
            if (aboutU != null)
            {
                _context.AboutUs.Remove(aboutU);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutUExists(decimal id)
        {
          return (_context.AboutUs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
