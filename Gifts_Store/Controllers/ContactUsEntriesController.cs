using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gifts_Store.Controllers
{
    public class ContactUsEntriesController : Controller
    {
        private readonly ModelContext _context;

        public ContactUsEntriesController(ModelContext context)
        {
            _context = context;
        }

        // GET: ContactUsEntries
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.ContactUsEntries.Include(c => c.Admin);
            return View(await modelContext.ToListAsync());
        }

        // GET: ContactUsEntries/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.ContactUsEntries == null)
            {
                return NotFound();
            }

            var contactUsEntry = await _context.ContactUsEntries
                .Include(c => c.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactUsEntry == null)
            {
                return NotFound();
            }

            return View(contactUsEntry);
        }

        // GET: ContactUsEntries/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id");
            return View();
        }

        // POST: ContactUsEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Subject,Message,AdminId")] ContactUsEntry contactUsEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactUsEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", contactUsEntry.AdminId);
            return View(contactUsEntry);
        }

        // GET: ContactUsEntries/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.ContactUsEntries == null)
            {
                return NotFound();
            }

            var contactUsEntry = await _context.ContactUsEntries.FindAsync(id);
            if (contactUsEntry == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", contactUsEntry.AdminId);
            return View(contactUsEntry);
        }

        // POST: ContactUsEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name,Email,Subject,Message,AdminId")] ContactUsEntry contactUsEntry)
        {
            if (id != contactUsEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactUsEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactUsEntryExists(contactUsEntry.Id))
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
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", contactUsEntry.AdminId);
            return View(contactUsEntry);
        }

        // GET: ContactUsEntries/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.ContactUsEntries == null)
            {
                return NotFound();
            }

            var contactUsEntry = await _context.ContactUsEntries
                .Include(c => c.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactUsEntry == null)
            {
                return NotFound();
            }

            return View(contactUsEntry);
        }

        // POST: ContactUsEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.ContactUsEntries == null)
            {
                return Problem("Entity set 'ModelContext.ContactUsEntries'  is null.");
            }
            var contactUsEntry = await _context.ContactUsEntries.FindAsync(id);
            if (contactUsEntry != null)
            {
                _context.ContactUsEntries.Remove(contactUsEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactUsEntryExists(decimal id)
        {
            return (_context.ContactUsEntries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
