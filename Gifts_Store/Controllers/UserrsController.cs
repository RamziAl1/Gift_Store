using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gifts_Store.Models;
using Microsoft.Extensions.Hosting;

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
            if(_context.Userrs == null)
                return Problem("Entity set 'ModelContext.Userrs'  is null.");
            if (_context.UserLogins == null)
                return Problem("Entity set 'ModelContext.UserLogins'  is null.");
            if (_context.Rolees == null)
                return Problem("Entity set 'ModelContext.Rolees'  is null.");
            var users = await (from u in _context.Userrs
                               join ul in _context.UserLogins on u.Id equals ul.UserId
                               join r in _context.Rolees on ul.RoleId equals r.Id
                               where r.Id != 1
                               select Tuple.Create(u, ul, r)).ToListAsync();

            return View(users);
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
			ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");
			return View();
        }

        // POST: Userrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _Create([Bind("Id,Fname,Lname,ImagePath,ImageFile,Status")] Userr userr)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string wwwRootPath = _webHostEnvironment.WebRootPath;
        //        // generate generate unique universal id
        //        string fileName = Guid.NewGuid().ToString() + "_" + userr.ImageFile.FileName;
        //        // define path to save file in
        //        string path = Path.Combine(wwwRootPath + "/images/", fileName);
        //        // save the file
        //        using (var filestream = new FileStream(path, FileMode.Create))
        //        {
        //            await userr.ImageFile.CopyToAsync(filestream);
        //        }
        //        // save path to model
        //        userr.ImagePath = fileName;

        //        _context.Add(userr);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(userr);
        //}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("UserName, Password, Email, Fname, Lname, ImageFile")] SignUpViewModel newUserData, string role, string catId)
		{
			await Console.Out.WriteLineAsync("In USer Create POST");

			ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");

			if (string.IsNullOrEmpty(role))
			{
				ModelState.AddModelError("RoleError", "Role is required.");
			}

			if (role != null && role == "2" && string.IsNullOrEmpty(catId))
			{
				ModelState.AddModelError("CatIdError", "Category is required.");
			}
			else
			{
				ModelState.Remove("catId");
			}

			if (ModelState.IsValid)
			{
				decimal? usernameExistsId = _context.UserLogins
					.Where(x => x.UserName == newUserData.UserName)
					.Select(x => x.UserId)
					.SingleOrDefault();
				if (usernameExistsId != null)
				{
					TempData["UsernameErrorMessage"] = "Username already exists.";
					return View();
				}

				decimal? emailExistsId = _context.UserLogins
					.Where(x => x.Email == newUserData.Email)
					.Select(x => x.UserId)
					.SingleOrDefault();
				if (emailExistsId != null)
				{
					TempData["EmailErrorMessage"] = "Email already exists.";
					return View();
				}

				string wwwRootPath = _webHostEnvironment.WebRootPath;
				string fileName = Guid.NewGuid().ToString() + "_" + newUserData.ImageFile.FileName;
				string path = Path.Combine(wwwRootPath + "/Images/", fileName);
				using (var filestream = new FileStream(path, FileMode.Create))
				{
					await newUserData.ImageFile.CopyToAsync(filestream);
				}
				int roleId = Int32.Parse(role);

				// save user entry
				Userr userModel = new Userr();
				userModel.Fname = newUserData.Fname;
				userModel.Lname = newUserData.Lname;
				userModel.ImagePath = fileName;
				userModel.Status = roleId == 3 ? "approved" : "pending";
				_context.Add(userModel);
				await _context.SaveChangesAsync();
				// save role dependant entry
				if (roleId == 2)
				{
					GiftMaker giftMaker = new GiftMaker();
					giftMaker.CategoryId = Int32.Parse(catId);
					giftMaker.UserId = userModel.Id;
					_context.Add(giftMaker);
					await _context.SaveChangesAsync();
				}
				else if (roleId == 3)
				{
					GiftSender giftSender = new GiftSender();
					giftSender.UserId = userModel.Id;
					_context.Add(giftSender);
					await _context.SaveChangesAsync();
				}
				await Console.Out.WriteLineAsync("roleid= " + roleId);
				// save the user_login entry 
				UserLogin login = new();
				login.RoleId = roleId;
				login.UserName = newUserData.UserName;
				login.Password = newUserData.Password;
				login.Email = newUserData.Email;
				login.UserId = userModel.Id;
				_context.Add(login);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "you have registered successfully";
				return RedirectToAction(nameof(Create));
			}

			await Console.Out.WriteLineAsync("data validation failed");
			return View();
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
