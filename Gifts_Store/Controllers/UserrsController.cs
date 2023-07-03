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
    public class UserrsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserrsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Userrs
        public async Task<IActionResult> Index()
        {
              return _context.Userrs != null ? 
                          View(await _context.Userrs.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Userrs'  is null.");
        }

        // GET: Userrs/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        // GET: Userrs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Userrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fname,Lname,ImagePath,ImageFile,Status")] Userr userr)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                // generate generate unique universal id
                string fileName = Guid.NewGuid().ToString() + "_" + userr.ImageFile.FileName;
                // define path to save file in
                string path = Path.Combine(wwwRootPath + "/images/", fileName);
                // save the file
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await userr.ImageFile.CopyToAsync(filestream);
                }
                // save path to model
                userr.ImagePath = fileName;

                _context.Add(userr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userr);
        }

        // GET: Userrs/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs.FindAsync(id);
            if (userr == null)
            {
                return NotFound();
            }
            return View(userr);
        }

        // POST: Userrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Fname,Lname,ImagePath,ImageFile,Status")] Userr userr)
        {
            if (id != userr.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if a new image was added
                    if (userr.ImageFile != null)
                    {
                        //delete old file

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        if (userr.ImagePath != null)
                        {
                            // define path of file to delete
                            string old_path = Path.Combine(wwwRootPath + "/images/", userr.ImagePath);
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
                        string fileName = Guid.NewGuid().ToString() + "_" + userr.ImageFile.FileName;
                        // define path to save file in
                        string new_path = Path.Combine(wwwRootPath + "/images/", fileName);
                        // save the file
                        using (var filestream = new FileStream(new_path, FileMode.Create))
                        {
                            await userr.ImageFile.CopyToAsync(filestream);
                        }
                        // save path to model
                        userr.ImagePath = fileName;
                    }
                    _context.Update(userr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserrExists(userr.Id))
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
            return View(userr);
        }

        // GET: Userrs/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        // POST: Userrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Userrs == null)
            {
                return Problem("Entity set 'ModelContext.Userrs'  is null.");
            }
            var userr = await _context.Userrs.FindAsync(id);
            if (userr != null)
            {
                if (userr.ImagePath != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // define path of file to delete
                    string path = Path.Combine(wwwRootPath + "/images/", userr.ImagePath);
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

                _context.Userrs.Remove(userr);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserrExists(decimal id)
        {
          return (_context.Userrs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
