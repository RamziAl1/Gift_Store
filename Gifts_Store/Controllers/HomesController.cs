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
    public class HomesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Homes
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Homes.Include(h => h.Admin);
            return View(await modelContext.ToListAsync());
        }

        //// Gets home page
        //public async Task<IActionResult> Home()
        //{
        //    var modelContext = _context.Homes.Include(h => h.Admin);
        //    return View(await modelContext.ToListAsync());
        //}

        // GET: Homes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes
                .Include(h => h.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }

        // GET: Homes/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id");
            return View();
        }

        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImageLogo,ImageBackground,SiteName,WelcomeText,AdminId")] Home home)
        {
            if (ModelState.IsValid)
            {
                await Console.Out.WriteLineAsync("------------------- model sate is valid");
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                // generate generate unique universal id
                string fileName1 = Guid.NewGuid().ToString() + "_" + home.ImageLogo.FileName;
                string fileName2 = Guid.NewGuid().ToString() + "_" + home.ImageBackground.FileName;
                // define path to save file in
                string path1 = Path.Combine(wwwRootPath + "/images/", fileName1);
                string path2 = Path.Combine(wwwRootPath + "/images/", fileName2);
                // save the file
                using (var filestream = new FileStream(path1, FileMode.Create))
                {
                    await home.ImageLogo.CopyToAsync(filestream);
                }
                using (var filestream = new FileStream(path2, FileMode.Create))
                {
                    await home.ImageBackground.CopyToAsync(filestream);
                }
                // save path to model
                home.LogoPath = fileName1;
                home.BackgroundPath = fileName2;

                home.AdminId = 1;
                //home.AdminId = GetHashCode from session

                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                await Console.Out.WriteLineAsync("------------------- model sate is NOT valid");
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", home.AdminId);
            return View(home);
        }

        // GET: Homes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes.FindAsync(id);
            if (home == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", home.AdminId);
            return View(home);
        }

        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,LogoPath,BackgroundPath,ImageLogo,ImageBackground,SiteName,WelcomeText,AdminId")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if a new logo image was added
                    if (home.ImageLogo != null)
                    {
                        //delete old file

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        if (home.LogoPath != null)
                        {
                            // define path of file to delete
                            string old_path = Path.Combine(wwwRootPath + "/images/", home.LogoPath);
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
                        string fileName = Guid.NewGuid().ToString() + "_" + home.ImageLogo.FileName;
                        // define path to save file in
                        string new_path = Path.Combine(wwwRootPath + "/images/", fileName);
                        // save the file
                        using (var filestream = new FileStream(new_path, FileMode.Create))
                        {
                            await home.ImageLogo.CopyToAsync(filestream);
                        }
                        // save path to model
                        home.LogoPath = fileName;
                    }

                    // check if a new background image was added
                    if (home.ImageBackground != null)
                    {
                        //delete old file

                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        if (home.BackgroundPath != null)
                        {
                            // define path of file to delete
                            string old_path = Path.Combine(wwwRootPath + "/images/", home.BackgroundPath);
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

                        string fileName = Guid.NewGuid().ToString() + "_" + home.ImageBackground.FileName;
                        // define path to save file in
                        string new_path = Path.Combine(wwwRootPath + "/images/", fileName);
                        // save the file
                        using (var filestream = new FileStream(new_path, FileMode.Create))
                        {
                            await home.ImageBackground.CopyToAsync(filestream);
                        }
                        // save path to model
                        home.BackgroundPath = fileName;
                    }

                    _context.Update(home);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeExists(home.Id))
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
            ViewData["AdminId"] = new SelectList(_context.Adminns, "Id", "Id", home.AdminId);
            return View(home);
        }

        // GET: Homes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes
                .Include(h => h.Admin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }

        // POST: Homes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Homes == null)
            {
                return Problem("Entity set 'ModelContext.Homes'  is null.");
            }
            var home = await _context.Homes.FindAsync(id);
            if (home != null)
            {
                // Delete logo
                if (home.LogoPath != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // define path of file to delete
                    string path = Path.Combine(wwwRootPath + "/images/", home.LogoPath);
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

                // Delete background image
                if (home.BackgroundPath != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // define path of file to delete
                    string path = Path.Combine(wwwRootPath + "/images/", home.BackgroundPath);
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

                _context.Homes.Remove(home);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomeExists(decimal id)
        {
          return (_context.Homes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
