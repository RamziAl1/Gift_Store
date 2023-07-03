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
    public class OrderrsController : Controller
    {
        private readonly ModelContext _context;

        public OrderrsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Orderrs
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Orderrs.Include(o => o.Gift).Include(o => o.GiftSender);
            return View(await modelContext.ToListAsync());
        }

        // GET: Orderrs/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Orderrs == null)
            {
                return NotFound();
            }

            var orderr = await _context.Orderrs
                .Include(o => o.Gift)
                .Include(o => o.GiftSender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderr == null)
            {
                return NotFound();
            }

            return View(orderr);
        }

        // GET: Orderrs/Create
        public IActionResult Create()
        {
            ViewData["GiftId"] = new SelectList(_context.Gifts, "Id", "Id");
            ViewData["GiftSenderId"] = new SelectList(_context.GiftSenders, "Id", "Id");
            return View();
        }

        // POST: Orderrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,ExpectedArrivalDate,Address,Quantity,Status,HasArrived,GiftSenderId,GiftId")] Orderr orderr)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiftId"] = new SelectList(_context.Gifts, "Id", "Id", orderr.GiftId);
            ViewData["GiftSenderId"] = new SelectList(_context.GiftSenders, "Id", "Id", orderr.GiftSenderId);
            return View(orderr);
        }

        // GET: Orderrs/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Orderrs == null)
            {
                return NotFound();
            }

            var orderr = await _context.Orderrs.FindAsync(id);
            if (orderr == null)
            {
                return NotFound();
            }
            ViewData["GiftId"] = new SelectList(_context.Gifts, "Id", "Id", orderr.GiftId);
            ViewData["GiftSenderId"] = new SelectList(_context.GiftSenders, "Id", "Id", orderr.GiftSenderId);
            return View(orderr);
        }

        // POST: Orderrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,OrderDate,ExpectedArrivalDate,Address,Quantity,Status,HasArrived,GiftSenderId,GiftId")] Orderr orderr)
        {
            if (id != orderr.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderrExists(orderr.Id))
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
            ViewData["GiftId"] = new SelectList(_context.Gifts, "Id", "Id", orderr.GiftId);
            ViewData["GiftSenderId"] = new SelectList(_context.GiftSenders, "Id", "Id", orderr.GiftSenderId);
            return View(orderr);
        }

        // GET: Orderrs/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Orderrs == null)
            {
                return NotFound();
            }

            var orderr = await _context.Orderrs
                .Include(o => o.Gift)
                .Include(o => o.GiftSender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderr == null)
            {
                return NotFound();
            }

            return View(orderr);
        }

        // POST: Orderrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Orderrs == null)
            {
                return Problem("Entity set 'ModelContext.Orderrs'  is null.");
            }
            var orderr = await _context.Orderrs.FindAsync(id);
            if (orderr != null)
            {
                _context.Orderrs.Remove(orderr);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderrExists(decimal id)
        {
          return (_context.Orderrs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
