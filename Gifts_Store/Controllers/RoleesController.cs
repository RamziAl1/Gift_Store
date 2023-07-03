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
    public class RoleesController : Controller
    {
        private readonly ModelContext _context;

        public RoleesController(ModelContext context)
        {
            _context = context;
        }

        // GET: Rolees
        public async Task<IActionResult> Index()
        {
              return _context.Rolees != null ? 
                          View(await _context.Rolees.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Rolees'  is null.");
        }

        // GET: Rolees/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Rolees == null)
            {
                return NotFound();
            }

            var rolee = await _context.Rolees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolee == null)
            {
                return NotFound();
            }

            return View(rolee);
        }

        // GET: Rolees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rolees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Rolee rolee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rolee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rolee);
        }

        // GET: Rolees/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Rolees == null)
            {
                return NotFound();
            }

            var rolee = await _context.Rolees.FindAsync(id);
            if (rolee == null)
            {
                return NotFound();
            }
            return View(rolee);
        }

        // POST: Rolees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name")] Rolee rolee)
        {
            if (id != rolee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleeExists(rolee.Id))
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
            return View(rolee);
        }

        // GET: Rolees/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Rolees == null)
            {
                return NotFound();
            }

            var rolee = await _context.Rolees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolee == null)
            {
                return NotFound();
            }

            return View(rolee);
        }

        // POST: Rolees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Rolees == null)
            {
                return Problem("Entity set 'ModelContext.Rolees'  is null.");
            }
            var rolee = await _context.Rolees.FindAsync(id);
            if (rolee != null)
            {
                _context.Rolees.Remove(rolee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleeExists(decimal id)
        {
          return (_context.Rolees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
