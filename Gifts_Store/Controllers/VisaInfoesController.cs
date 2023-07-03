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
    public class VisaInfoesController : Controller
    {
        private readonly ModelContext _context;

        public VisaInfoesController(ModelContext context)
        {
            _context = context;
        }

        // GET: VisaInfoes
        public async Task<IActionResult> Index()
        {
              return _context.VisaInfos != null ? 
                          View(await _context.VisaInfos.ToListAsync()) :
                          Problem("Entity set 'ModelContext.VisaInfos'  is null.");
        }

        // GET: VisaInfoes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.VisaInfos == null)
            {
                return NotFound();
            }

            var visaInfo = await _context.VisaInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visaInfo == null)
            {
                return NotFound();
            }

            return View(visaInfo);
        }

        // GET: VisaInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VisaInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardHolderName,CardNumber,Ccv,ExpireDate,Balance")] VisaInfo visaInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visaInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(visaInfo);
        }

        // GET: VisaInfoes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.VisaInfos == null)
            {
                return NotFound();
            }

            var visaInfo = await _context.VisaInfos.FindAsync(id);
            if (visaInfo == null)
            {
                return NotFound();
            }
            return View(visaInfo);
        }

        // POST: VisaInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,CardHolderName,CardNumber,Ccv,ExpireDate,Balance")] VisaInfo visaInfo)
        {
            if (id != visaInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visaInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisaInfoExists(visaInfo.Id))
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
            return View(visaInfo);
        }

        // GET: VisaInfoes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.VisaInfos == null)
            {
                return NotFound();
            }

            var visaInfo = await _context.VisaInfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visaInfo == null)
            {
                return NotFound();
            }

            return View(visaInfo);
        }

        // POST: VisaInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.VisaInfos == null)
            {
                return Problem("Entity set 'ModelContext.VisaInfos'  is null.");
            }
            var visaInfo = await _context.VisaInfos.FindAsync(id);
            if (visaInfo != null)
            {
                _context.VisaInfos.Remove(visaInfo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisaInfoExists(decimal id)
        {
          return (_context.VisaInfos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
