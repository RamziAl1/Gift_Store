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
    public class GiftSendersController : Controller
    {
        private readonly ModelContext _context;

        public GiftSendersController(ModelContext context)
        {
            _context = context;
        }

        // GET: GiftSenders
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.GiftSenders.Include(g => g.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: GiftSenders/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.GiftSenders == null)
            {
                return NotFound();
            }

            var giftSender = await _context.GiftSenders
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giftSender == null)
            {
                return NotFound();
            }

            return View(giftSender);
        }

        // GET: GiftSenders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id");
            return View();
        }

        // POST: GiftSenders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] GiftSender giftSender)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giftSender);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftSender.UserId);
            return View(giftSender);
        }

        // GET: GiftSenders/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.GiftSenders == null)
            {
                return NotFound();
            }

            var giftSender = await _context.GiftSenders.FindAsync(id);
            if (giftSender == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftSender.UserId);
            return View(giftSender);
        }

        // POST: GiftSenders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,UserId")] GiftSender giftSender)
        {
            if (id != giftSender.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giftSender);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftSenderExists(giftSender.Id))
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
            ViewData["UserId"] = new SelectList(_context.Userrs, "Id", "Id", giftSender.UserId);
            return View(giftSender);
        }

        // GET: GiftSenders/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.GiftSenders == null)
            {
                return NotFound();
            }

            var giftSender = await _context.GiftSenders
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (giftSender == null)
            {
                return NotFound();
            }

            return View(giftSender);
        }

        // POST: GiftSenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.GiftSenders == null)
            {
                return Problem("Entity set 'ModelContext.GiftSenders'  is null.");
            }
            var giftSender = await _context.GiftSenders.FindAsync(id);
            if (giftSender != null)
            {
                _context.GiftSenders.Remove(giftSender);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiftSenderExists(decimal id)
        {
          return (_context.GiftSenders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
