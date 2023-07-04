using Gifts_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Controllers
{
    public class AuthController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;

        public AuthController(ModelContext context, IWebHostEnvironment ihe)
        {
            _context = context;
            _environment = ihe;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            Console.WriteLine("In Register Get");
            ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register([Bind("UserName, Password, Email, Fname, Lname, ImageFile")] SignUpViewModel newUserData, string role, string catId)
        {
            await Console.Out.WriteLineAsync("In Register POST");

			ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");

			if (string.IsNullOrEmpty(role))
			{
				ModelState.AddModelError("RoleError", "Role is required.");
			}

			if (string.IsNullOrEmpty(catId))
			{
				ModelState.AddModelError("CatIdError", "Category is required.");
			}


			if (ModelState.IsValid)
            {
				decimal? usernameExistsId = _context.UserLogins
                    .Where(x => x.UserName == newUserData.UserName)
                    .Select(x => x.UserId)
                    .SingleOrDefault();
                if(usernameExistsId != null)
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

                string wwwRootPath = _environment.WebRootPath;
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
                return RedirectToAction(nameof(Register));
            }

            await Console.Out.WriteLineAsync("data validation failed");
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		//[ValidateAntiForgeryToken]
		[AllowAnonymous]
        public IActionResult Login([Bind("UserName, Password")] UserLogin uLogin)
        {
            ModelState.Remove("Email");
			foreach (var key in ModelState.Keys)
			{
				if (ModelState.TryGetValue(key, out var entry) && entry.Errors.Count > 0)
				{
					foreach (var error in entry.Errors)
					{
						var errorMessage = error.ErrorMessage;
						var attributeName = key;

						Console.WriteLine($"Error: {errorMessage} (Attribute: {attributeName})");
					}
				}
			}
            Console.WriteLine("------------in login post---------------");
            if (ModelState.IsValid)
            {
                var auth = _context.UserLogins.Where(x => x.UserName == uLogin.UserName && x.Password == uLogin.Password).SingleOrDefault();
                if (auth != null)
                {
                    var user = (from ul in _context.UserLogins
                                join u in _context.Userrs on ul.UserId equals u.Id
                                where ul.Id == auth.Id
                                select Tuple.Create(u.Status, u.Id)).FirstOrDefault();

                    if (user?.Item1 == "pending")
                    {
                        TempData["ErrorMessage"] = "User account hasn't been approved yet.";
                        return View();
                    }

					HttpContext.Session.SetInt32("userLoginId", (int)auth.Id);
					HttpContext.Session.SetString("Username", auth.UserName);
					HttpContext.Session.SetInt32("UserId", (int)user.Item2);
					HttpContext.Session.SetInt32("RoleId", (int)auth.RoleId);

					switch (auth.RoleId)
                    {
                        case 1:
                            var adminId = (from ul in _context.UserLogins
										   join u in _context.Userrs on ul.UserId equals u.Id
										   join a in _context.Adminns on u.Id equals a.UserId
										   where ul.Id == auth.Id
										   select a.Id).FirstOrDefault();
                            HttpContext.Session.SetInt32("AdminId", (int)adminId);

                            return RedirectToAction("Dashboard", "Admin");

                        case 2:
							var makerId = (from ul in _context.UserLogins
										   join u in _context.Userrs on ul.UserId equals u.Id
										   join gm in _context.GiftMakers on u.Id equals gm.UserId
										   where ul.Id == auth.Id
                                           select gm.Id).FirstOrDefault();
                            Console.WriteLine("in login makerId= " + makerId );
							HttpContext.Session.SetInt32("MakerId", (int)makerId);

							return RedirectToAction("Home", "Home");

						case 3:
							var senderId = (from ul in _context.UserLogins
										   join u in _context.Userrs on ul.UserId equals u.Id
										   join gs in _context.GiftSenders on u.Id equals gs.UserId
										   where ul.Id == auth.Id
                                            select gs.Id).FirstOrDefault();
							HttpContext.Session.SetInt32("SenderId", (int)senderId);

							return RedirectToAction("Home", "Home");
					}
					
				}
                else
                    TempData["ErrorMessage"] = "Invalid username or password.";
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult LoginAsGuest()
        {
			HttpContext.Session.SetInt32("RoleId", 4);
			return RedirectToAction("Home", "Home");
        }

        public IActionResult Logout() 
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
