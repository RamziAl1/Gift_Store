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
    public class AdminnsController : Controller
    {
        private readonly ModelContext _context;

        public AdminnsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Adminns
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Adminns.Include(a => a.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Adminns/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Adminns == null)
            {
                return NotFound();
            }

            var adminn = await _context.Adminns
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminn == null)
            {
                return NotFound();
            }

            return View(adminn);
        }

        // GET: Adminns/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id");
            return View();
        }

        // POST: Adminns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Profit,UserId")] Adminn adminn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", adminn.UserId);
            return View(adminn);
        }

        // GET: Adminns/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Adminns == null)
            {
                return NotFound();
            }

            var adminn = await _context.Adminns.FindAsync(id);
            if (adminn == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", adminn.UserId);
            return View(adminn);
        }

        // POST: Adminns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Profit,UserId")] Adminn adminn)
        {
            if (id != adminn.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminnExists(adminn.Id))
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
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", adminn.UserId);
            return View(adminn);
        }

        // GET: Adminns/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Adminns == null)
            {
                return NotFound();
            }

            var adminn = await _context.Adminns
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adminn == null)
            {
                return NotFound();
            }

            return View(adminn);
        }

        // POST: Adminns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Adminns == null)
            {
                return Problem("Entity set 'ModelContext.Adminns'  is null.");
            }
            var adminn = await _context.Adminns.FindAsync(id);
            if (adminn != null)
            {
                _context.Adminns.Remove(adminn);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminnExists(decimal id)
        {
          return (_context.Adminns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
