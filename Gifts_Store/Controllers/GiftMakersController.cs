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
    public class GiftMakersController : Controller
    {
        private readonly ModelContext _context;

        public GiftMakersController(ModelContext context)
        {
            _context = context;
        }

        // GET: GiftMakers
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.GiftMakers.Include(g => g.Category).Include(g => g.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: GiftMakers/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.GiftMakers == null)
            {
                return NotFound();
            }

            var giftMaker = await _context.GiftMakers
                .Include(g => g.Category)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giftMaker == null)
            {
                return NotFound();
            }

            return View(giftMaker);
        }

        // GET: GiftMakers/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id");
            return View();
        }

        // POST: GiftMakers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Profit,UserId,CategoryId")] GiftMaker giftMaker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giftMaker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", giftMaker.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftMaker.UserId);
            return View(giftMaker);
        }

        // GET: GiftMakers/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.GiftMakers == null)
            {
                return NotFound();
            }

            var giftMaker = await _context.GiftMakers.FindAsync(id);
            if (giftMaker == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", giftMaker.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftMaker.UserId);
            return View(giftMaker);
        }

        // POST: GiftMakers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Profit,UserId,CategoryId")] GiftMaker giftMaker)
        {
            if (id != giftMaker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giftMaker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftMakerExists(giftMaker.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", giftMaker.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftMaker.UserId);
            return View(giftMaker);
        }

        // GET: GiftMakers/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.GiftMakers == null)
            {
                return NotFound();
            }

            var giftMaker = await _context.GiftMakers
                .Include(g => g.Category)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giftMaker == null)
            {
                return NotFound();
            }

            return View(giftMaker);
        }

        // POST: GiftMakers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.GiftMakers == null)
            {
                return Problem("Entity set 'ModelContext.GiftMakers'  is null.");
            }
            var giftMaker = await _context.GiftMakers.FindAsync(id);
            if (giftMaker != null)
            {
                _context.GiftMakers.Remove(giftMaker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiftMakerExists(decimal id)
        {
          return (_context.GiftMakers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
