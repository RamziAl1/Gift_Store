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
    public class GiftsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GiftsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Gifts
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Gifts.Include(g => g.Category).Include(g => g.GiftMaker);
            return View(await modelContext.ToListAsync());
        }

        // GET: Gifts/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Gifts == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.GiftMaker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gift == null)
            {
                return NotFound();
            }

            return View(gift);
        }

        // GET: Gifts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            ViewData["GiftMakerId"] = new SelectList(_context.GiftMakers, "Id", "Id");
            return View();
        }

        // POST: Gifts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Sale,Price,Quantity,ImagePath,ImageFile,AddedDate,CategoryId,GiftMakerId")] Gift gift)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                // generate generate unique universal id
                string fileName = Guid.NewGuid().ToString() + "_" + gift.ImageFile.FileName;
                // define path to save file in
                string path = Path.Combine(wwwRootPath + "/images/", fileName);
                // save the file
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await gift.ImageFile.CopyToAsync(filestream);
                }
                // save path to model
                gift.ImagePath = fileName;

                _context.Add(gift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", gift.CategoryId);
            ViewData["GiftMakerId"] = new SelectList(_context.GiftMakers, "Id", "Id", gift.GiftMakerId);
            return View(gift);
        }

        // GET: Gifts/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Gifts == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", gift.CategoryId);
            ViewData["GiftMakerId"] = new SelectList(_context.GiftMakers, "Id", "Id", gift.GiftMakerId);
            return View(gift);
        }

        // POST: Gifts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name,Sale,Price,Quantity,ImagePath,ImageFile,AddedDate,CategoryId,GiftMakerId")] Gift gift)
        {
            if (id != gift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if a new image was added
                    if (gift.ImageFile != null)
                    {
                        //delete old file

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        if (gift.ImagePath != null)
                        {
                            // define path of file to delete
                            string old_path = Path.Combine(wwwRootPath + "/images/", gift.ImagePath);
                            // delete the file
                            try
                            {
                                // Check if the file exists before deleting
                                if (System.IO.File.Exists(old_path))
                                    System.IO.File.Delete(old_path);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
                            }
                        }


                        // add new file

                        string fileName = Guid.NewGuid().ToString() + "_" + gift.ImageFile.FileName;
                        // define path to save file in
                        string new_path = Path.Combine(wwwRootPath + "/images/", fileName);
                        // save the file
                        using (var filestream = new FileStream(new_path, FileMode.Create))
                        {
                            await gift.ImageFile.CopyToAsync(filestream);
                        }
                        // save path to model
                        gift.ImagePath = fileName;
                    }

                    _context.Update(gift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftExists(gift.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", gift.CategoryId);
            ViewData["GiftMakerId"] = new SelectList(_context.GiftMakers, "Id", "Id", gift.GiftMakerId);
            return View(gift);
        }

        // GET: Gifts/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Gifts == null)
            {
                return NotFound();
            }

            var gift = await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.GiftMaker)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gift == null)
            {
                return NotFound();
            }

            return View(gift);
        }

        // POST: Gifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Gifts == null)
            {
                return Problem("Entity set 'ModelContext.Gifts'  is null.");
            }
            var gift = await _context.Gifts.FindAsync(id);
            if (gift != null)
            {
                if (gift.ImagePath != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // define path of file to delete
                    string path = Path.Combine(wwwRootPath + "/images/", gift.ImagePath);
                    // delete the file
                    try
                    {
                        // Check if the file exists before deleting
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
                    }
                }

                _context.Gifts.Remove(gift);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiftExists(decimal id)
        {
          return (_context.Gifts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
