using Gifts_Store.Enums;
using Gifts_Store.Models;
using Gifts_Store.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Gifts_Store.Controllers
{
    public class GiftMakerController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;

        public GiftMakerController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult MyProducts()
        {
			int? giftMakerId = HttpContext.Session.GetInt32("MakerId");

            var giftMaker = _context.GiftMakers
            .Include(x => x.Category)
            .SingleOrDefault(x => x.Id == giftMakerId);

			var categoryName = giftMaker?.Category?.CategoryName ?? "Default Category";

            var gifts = _context.Gifts.Include(x => x.GiftMaker)
                .Where(x => x.GiftMakerId == giftMakerId)
                .AsEnumerable();

            return View(Tuple.Create(gifts, categoryName));
        }

        public IActionResult AddGift()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGift([Bind("Name,Sale,Price,Quantity,ImageFile")] Gift gift)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _environment.WebRootPath;
                // generate generate unique universal id
                string fileName = Guid.NewGuid().ToString() + "_" + gift?.ImageFile?.FileName;
                // define path to save file in
                string path = Path.Combine(wwwRootPath + "/images/", fileName);
                // save the file
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    await gift.ImageFile.CopyToAsync(filestream);
                }
                // save path to model
                gift.ImagePath = fileName;
                var makerId = HttpContext.Session.GetInt32("MakerId");
                gift.GiftMakerId = (decimal?)makerId;
                gift.AddedDate = DateTime.Now.Date;
                gift.CategoryId = _context.GiftMakers.SingleOrDefault(x => x.Id == makerId)?.CategoryId;

                _context.Add(gift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyProducts));
            }
            return View();
        }

        public async Task<IActionResult> GiftDetails(decimal? id)
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

        public async Task<IActionResult> EditGift(decimal? id)
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

            return View(gift);
        }

        [HttpPost]
        public async Task<IActionResult> EditGift([Bind("Id,Name,Sale,Price,Quantity,ImagePath,ImageFile,AddedDate,CategoryId,GiftMakerId")] Gift gift)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check if a new image was added
                    if (gift.ImageFile != null)
                    {
                        //delete old file

                        string wwwRootPath = _environment.WebRootPath;
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
                    throw new DbUpdateConcurrencyException();
                }
                return RedirectToAction(nameof(MyProducts));
            }
            return View(gift);
        }

        public async Task<IActionResult> DeleteGift(decimal? id)
        {
            if (id == null || _context.Gifts == null)
            {
                return Problem("Entity set 'ModelContext.Gifts'  is null.");
            }

            var gift = await _context.Gifts.FindAsync(id);
            if (gift != null)
            {
                if (gift.ImagePath != null)
                {
                    string wwwRootPath = _environment.WebRootPath;
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
            return RedirectToAction(nameof(MyProducts));
        }

        public IActionResult MyOrders()
        {
            var giftMakerId = HttpContext.Session.GetInt32("MakerId");
            var query = from gm in _context.GiftMakers
                        join g in _context.Gifts on gm.Id equals g.GiftMakerId
                        join o in _context.Orderrs on g.Id equals o.GiftId
                        where gm.Id == giftMakerId && o.Status != "in cart"
                        orderby o.Status descending, o.OrderDate descending
                        select Tuple.Create(o, g);
            var model = query.AsEnumerable();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveGiftRequest(decimal id)
        {
            if (_context.Orderrs == null)
            {
                return Problem("Entity set 'ModelContext.Orderrs' is null.");
            }
            var orderr = await _context.Orderrs.FindAsync(id);

            if (orderr != null)
            {
                orderr.Status = "approved";
                _context.Update(orderr);
            }
            else
            {
                return Problem("Specific order not found");
            }

            var senderInfo = await 
                            (from o in _context.Orderrs
                             join gs in _context.GiftSenders on o.GiftSenderId equals gs.Id
                             join u in _context.Userrs on gs.UserId equals u.Id
                             join ul  in _context.UserLogins on u.Id equals ul.UserId
                             where o.Id == orderr.Id
                             select Tuple.Create(u, ul)).SingleOrDefaultAsync();

            if(senderInfo == null)
                return Problem("User and user login for order not found");

            var gift = await _context.Gifts.FindAsync(orderr.GiftId);
            

            if (gift == null)
                return Problem("Gift for order not found");

            if(gift.Quantity < orderr.Quantity)
            {
                TempData["QuantityNotEnoughmessage" + orderr.Id] = "Not enough of that item in stock, please restock before approving the request";
                return RedirectToAction(nameof(MyOrders));
            }
            gift.Quantity = gift.Quantity - orderr.Quantity;
            _context.Update(gift);

            Task.Run(() =>
            {
                string recipientName = $"{senderInfo?.Item1.Fname} {senderInfo?.Item1.Lname}";
                string giftName = gift.Name;
                string orderQuantity = orderr.Quantity.ToString();
                string orderAddress = orderr.Address;
                string orderArrivalDate = orderr.ExpectedArrivalDate.ToString();
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.GiftRequestApproval.GetSubject();
                string emailBody = EmailTemplate.GiftRequestApproval.GetBody(recipientName, giftName, orderQuantity, orderAddress, orderArrivalDate, senderName);
                string? toEmail = senderInfo?.Item2?.Email;

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, toEmail);
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyOrders));

        }

        [HttpPost]
        public async Task<IActionResult> RejectGiftRequest(decimal id)
        {
            if (_context.Orderrs == null)
            {
                return Problem("Entity set 'ModelContext.Orderrs' is null.");
            }
            var orderr = await _context.Orderrs.FindAsync(id);

            if (orderr != null)
            {
                _context.Orderrs.Remove(orderr);
            }
            else
            {
                return Problem("Specific order not found");
            }

            var senderInfo = await
                            (from o in _context.Orderrs
                             join gs in _context.GiftSenders on o.GiftSenderId equals gs.Id
                             join u in _context.Userrs on gs.UserId equals u.Id
                             join ul in _context.UserLogins on u.Id equals ul.UserId
                             where o.Id == orderr.Id
                             select Tuple.Create(u, ul)).SingleOrDefaultAsync();

            if (senderInfo == null)
                return Problem("User and user login for order not found");

            var gift = await _context.Gifts.FindAsync(orderr.GiftId);

            if (gift == null)
                return Problem("Gift for order not found");

            Task.Run(() =>
            {
                string recipientName = $"{senderInfo?.Item1.Fname} {senderInfo?.Item1.Lname}";
                string giftName = gift.Name;
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.GiftRequestRejection.GetSubject();
                string emailBody = EmailTemplate.GiftRequestRejection.GetBody(recipientName, giftName, senderName);
                string? toEmail = senderInfo?.Item2?.Email;

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, toEmail);
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyOrders));
        }

        [HttpPost]
        public async Task<IActionResult> SetOrderArrived(decimal? id)
        {
            if (id == null)
                return NotFound("Order Id is null");
            var order = await _context.Orderrs.SingleOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound("Order not found based on orderId");
            order.HasArrived = true;

            var senderInfo = await
                            (from o in _context.Orderrs
                             join gs in _context.GiftSenders on o.GiftSenderId equals gs.Id
                             join u in _context.Userrs on gs.UserId equals u.Id
                             join ul in _context.UserLogins on u.Id equals ul.UserId
                             where o.Id == order.Id
                             select Tuple.Create(u, ul)).SingleOrDefaultAsync();

            if (senderInfo == null)
                return Problem("User and user login for order not found");

            var gift = await _context.Gifts.FindAsync(order.GiftId);

            if (gift == null)
                return Problem("Gift for order not found");

            Task.Run(() =>
            {
                string recipientName = $"{senderInfo?.Item1.Fname} {senderInfo?.Item1.Lname}";
                string orderDate = order.OrderDate.Date.ToString("yyyy-MM-dd");
                string expectedArrivalDate = order.ExpectedArrivalDate.Date.ToString("yyyy-MM-dd");
                string orderAddress = order.Address;
                string giftName = gift.Name;
                string orderQuantity = order.Quantity.ToString();
                string orderTotalPrice = order.TotalPrice.ToString();
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.OrderArrival.GetSubject();
                string emailBody = EmailTemplate.OrderArrival.GetBody(recipientName, orderDate, expectedArrivalDate, orderAddress, giftName, orderQuantity, orderTotalPrice, senderName);
                string? toEmail = senderInfo?.Item2?.Email;

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, toEmail);
            });

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> MyTestimonial()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null || _context.Testimonials == null)
			{
				return NotFound();
			}

            var testimonial = await _context.Testimonials
                .SingleOrDefaultAsync(m => m.UserId == userId);

            if (testimonial == null)
			{
				return View();
			}

			return View(testimonial);
        }

		[HttpPost]
		public async Task<IActionResult> Mytestimonial([Bind("Id,Messege")] Testimonial testimonial)
		{
			if (ModelState.IsValid)
			{
                testimonial.Status = "pending";
                testimonial.UserId = (decimal?)HttpContext.Session.GetInt32("UserId");
				_context.Add(testimonial);
				await _context.SaveChangesAsync();
				return View(testimonial);
			}
			
			return View();
		}

		public IActionResult MyProfile()
        {
            if (HttpContext != null && HttpContext.Session != null)
            {
                int? userLoginId = HttpContext?.Session?.GetInt32("userLoginId");
                var model = _context.UserLogins
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Where(x => x.Id == userLoginId)
                    .SingleOrDefault();

                if (model == null)
                    return NotFound();

                var giftMaker = _context.GiftMakers.Include(x => x.Category).FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("MakerId"));
                if (giftMaker == null || giftMaker.Category == null)
                    return NotFound();

                ViewBag.categoryName = giftMaker.Category.CategoryName;

                return View(model);
            }
            return View();
        }

        public IActionResult EditProfile()
        {
            ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");
            var giftMaker = _context.GiftMakers.Include(x => x.Category).FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("MakerId"));
            if (giftMaker == null || giftMaker.Category == null)
                return NotFound();
            ViewBag.currentCategoryId = giftMaker.CategoryId;
            ViewBag.currentCategory = giftMaker.Category.CategoryName;
            int? uLoginId = HttpContext.Session.GetInt32("userLoginId");
            var query = from u in _context.Userrs
                        join ul in _context.UserLogins on u.Id equals ul.UserId
                        where ul.Id == (decimal?)uLoginId
                        select u;

            var user = query.SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve error messages from session
            if (HttpContext.Session.TryGetValue("ErrorMessages", out byte[] errorMessagesBytes))
            {
                string errorMessagesJson = Encoding.UTF8.GetString(errorMessagesBytes);
                List<string> errorMessages = JsonConvert.DeserializeObject<List<string>>(errorMessagesJson);

                // Use the error messages as needed
                ViewBag.ErrorMessages = errorMessages;

                // Clear the error messages from session
                HttpContext.Session.Remove("ErrorMessages");
            }

            string? invalidPasswordMessage = TempData["InvalidPasswordMessage"] as string;
            ViewBag.InvalidPasswordMessage = invalidPasswordMessage;
            
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile([Bind("Id,Status,Fname,Lname,ImagePath,ImageFile")] Userr user, string catId)
        {
            ViewData["CategoryNames"] = new SelectList(_context.Categories, "Id", "CategoryName");
            var giftMaker = _context.GiftMakers.Include(x => x.Category).FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("MakerId"));
            if (giftMaker == null || giftMaker.Category == null)
                return NotFound();
            

            if (string.IsNullOrEmpty(catId))
            {
                ModelState.AddModelError("CategoryError", "Category is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if a new image was added
                    if (user.ImageFile != null)
                    {
                        //delete old file

                        string wwwRootPath = _environment.WebRootPath;
                        if (user.ImagePath != null)
                        {
                            // define path of file to delete
                            string old_path = Path.Combine(wwwRootPath + "/images/", user.ImagePath);
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
                        string fileName = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                        // define path to save file in
                        string new_path = Path.Combine(wwwRootPath + "/images/", fileName);
                        // save the file
                        using (var filestream = new FileStream(new_path, FileMode.Create))
                        {
                            await user.ImageFile.CopyToAsync(filestream);
                        }
                        // save path to model
                        user.ImagePath = fileName;
                    }
                    giftMaker.CategoryId = decimal.Parse(catId);
                    _context.Update(giftMaker);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    ViewBag.currentCategoryId = giftMaker.CategoryId;
                    ViewBag.currentCategory = _context.Categories.SingleOrDefault(x => x.Id == giftMaker.CategoryId).CategoryName;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditPassword(string oldPassword,
            [Required(ErrorMessage = "new password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d\s:]).{8,}$",
                ErrorMessage = "new password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
            string newPassword)
        {

            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            HttpContext.Session.Set("ErrorMessages", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(errorMessages)));

            if (ModelState.IsValid)
            {
                string? username = HttpContext.Session.GetString("Username");
                var userLogin = _context.UserLogins.Where(x => x.UserName == username && x.Password == oldPassword).SingleOrDefault();
                if (userLogin != null)
                {
                    userLogin.Password = newPassword;
                    _context.Update(userLogin);
                    await _context.SaveChangesAsync();
                }
                else
                    TempData["InvalidPasswordMessage"] = "Invalid old password.";
            }
            
            return RedirectToAction("EditProfile", "GiftMaker");
        }


    }
}
