﻿using Gifts_Store.Constants;
using Gifts_Store.Enums;
using Gifts_Store.Models;
using Gifts_Store.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gifts_Store.Controllers
{
    public class GiftSenderController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;

        public GiftSenderController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

		public IActionResult BrowseGifts()
        {
			var gifts = _context.Gifts
				.AsEnumerable();

			ViewData["Categories"] = new SelectList(_context.Categories, "Id", "CategoryName");
			return View(gifts);
        }

        [HttpPost]
        public async Task<IActionResult> BrowseGifts(string giftName, string giftCategory)
        {
            await Console.Out.WriteLineAsync("----- giftName=" + giftName + "  ---  giftCategory=" + giftCategory + "  --------");
			ViewData["Categories"] = new SelectList(_context.Categories, "Id", "CategoryName");

			if ((giftName == null || giftName == "") && giftCategory == "All")
            {
				var gifts = _context.Gifts
				    .AsEnumerable();
                return View(gifts);
			}
            else if((giftName == null || giftName == "") && giftCategory != "All")
            {
                var categoryId = Int32.Parse(giftCategory);
				var gifts = _context.Gifts
                    .Where(x => x.CategoryId == categoryId)
				    .AsEnumerable();
				return View(gifts);
			}
			else if ((giftName != null && giftName != "") && giftCategory == "All")
			{
				var gifts = _context.Gifts
					.Where(x => x.Name.ToLower().Contains(giftName.ToLower()))
					.AsEnumerable();
				return View(gifts);
			}
			else if ((giftName != null && giftName != "") && giftCategory != "All")
			{
				var categoryId = Int32.Parse(giftCategory);
				var gifts = _context.Gifts
					.Where(x => x.Name.ToLower().Contains(giftName.ToLower()) && x.CategoryId == categoryId)
					.AsEnumerable();
				return View(gifts);
			}

			return View();
        }

        public async Task<IActionResult> OrderRequest(decimal? id)
        {
            if (id == null || _context.Gifts == null)
            {
                return NotFound();
            }
            var gift = await _context.Gifts.SingleOrDefaultAsync(x => x.Id == id);
            if (gift == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("requestGiftId", (int)id);
            Orderr? order = new Orderr();
            return View(Tuple.Create(order, gift));
        }

        [HttpPost]
        public async Task<IActionResult> OrderRequest([Bind(Prefix ="Item1")] Orderr orderr)
        {
            var giftId = HttpContext.Session.GetInt32("requestGiftId");
            if (giftId == null)
                return NotFound("giftId is null");
            HttpContext.Session.Remove("requestGiftId");
            var gift = await _context.Gifts.SingleOrDefaultAsync(x => x.Id == giftId);
            if (gift == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                orderr.TotalPrice = orderr.Quantity * (gift.Price * ( 1 - gift.Sale/100));
                orderr.GiftId = giftId;
                orderr.GiftSenderId = HttpContext.Session.GetInt32("SenderId");
                orderr.OrderDate = DateTime.Now.Date;
                orderr.ExpectedArrivalDate = DateTime.Now.Date.AddDays(30);
                orderr.Status = "pending";
                orderr.HasArrived = false;
                orderr.PaymentMade = false;

                _context.Add(orderr);
                await _context.SaveChangesAsync();

                TempData["OrderRequestStatus"] = "Order Successful";

                return RedirectToAction(nameof(BrowseGifts));
            }

            return View(Tuple.Create(orderr, gift));
        }

        public IActionResult MyOrders()
        {
            
            var senderId = HttpContext.Session.GetInt32("SenderId");
            var query = from gs in _context.GiftSenders
                        join o in _context.Orderrs on gs.Id equals o.GiftSenderId
                        join g in _context.Gifts on o.GiftId equals g.Id
                        where gs.Id == senderId
                        orderby o.Status ascending
                        select Tuple.Create(o, g);
            var model = query.AsEnumerable();
            return View(model);
        }

        public async Task<IActionResult> MakePayment(decimal? id)
        {
            if (id == null)
                return NotFound("passed orderId is null in controller action");

            HttpContext.Session.SetInt32("PaymentOrderId", (int)id);
            VisaInfo visaInfo = new VisaInfo();
            var query = from gs in _context.GiftSenders
                        join o in _context.Orderrs on gs.Id equals o.GiftSenderId
                        join g in _context.Gifts on o.GiftId equals g.Id
                        where o.Id == id
                        select Tuple.Create(o, g, visaInfo);
            var model = await query.SingleOrDefaultAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment([Bind(Prefix = "Item3")] VisaInfo visaInfo)
        {
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
            var orderId = HttpContext.Session.GetInt32("PaymentOrderId");
            var query = from gs in _context.GiftSenders
                        join o in _context.Orderrs on gs.Id equals o.GiftSenderId
                        join g in _context.Gifts on o.GiftId equals g.Id
                        where o.Id == orderId
                        select Tuple.Create(o, g, visaInfo);

            var model = await query.SingleOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var visaInfoDB = await _context.VisaInfos
                    .SingleOrDefaultAsync(x => x.CardNumber == visaInfo.CardNumber
                    && x.CardHolderName.ToLower() == visaInfo.CardHolderName.ToLower()
                    && x.Ccv == visaInfo.Ccv
                    && x.ExpireDate.Year == visaInfo.ExpireDate.Year 
                    && x.ExpireDate.Month == visaInfo.ExpireDate.Month
                    );
                
                if (visaInfoDB == null)
                {
                    ViewBag.ErrorMessage = "Card doesn't exist, please try again";
                    return View(model);
                }

                if (visaInfo.ExpireDate < DateTime.Now)
                {
                    ViewBag.ErrorMessage = $"Card expired on {visaInfo.ExpireDate}, please enter a valid card";
                    return View(model);
                }

                if (model != null && model.Item1.TotalPrice > visaInfoDB?.Balance)
                {
                    ViewBag.ErrorMessage = $"Card has insufficient balance for transaction (Card balance=${visaInfoDB.Balance})" +
                        $"(Transaction amount=${model.Item1.TotalPrice}), please enter a card with enough balance";
                    return View(model);
                }

                var adminn = await _context.Adminns.SingleOrDefaultAsync(x => x.Id == 1);
                var giftMaker = await _context.GiftMakers.SingleOrDefaultAsync(x => x.Id == model.Item2.GiftMakerId);

                visaInfoDB.Balance = visaInfoDB.Balance - model.Item1.TotalPrice;
                adminn.Profit = adminn.Profit + ProfitDistribution.AdminPercentage * model.Item1.TotalPrice;
                giftMaker.Profit = giftMaker.Profit + ProfitDistribution.GiftMakerPercentage * model.Item1.TotalPrice;
                model.Item1.PaymentMade = true;

                TempData["TransactionSuccessStatus"] = "payment successful";

                _context.Update(visaInfoDB);
                _context.Update(adminn);
                _context.Update(giftMaker);
                _context.Update(model.Item1);
                await _context.SaveChangesAsync();

                var userId = HttpContext.Session.GetInt32("UserId");
                var userr = await _context.Userrs.SingleOrDefaultAsync(x => x.Id == userId);
                var userLogin = await _context.UserLogins.SingleOrDefaultAsync(x => x.UserId == userId);

                string recipientName = $"{userr?.Fname} {userr?.Lname}";
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.InvoiceBody.GetSubject();
                string emailBody = EmailTemplate.InvoiceBody.GetBody(recipientName, senderName);

                string orderDate = model.Item1.OrderDate.Date.ToString("yyyy-MM-dd");
                string paymentDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                string giftName = model.Item2.Name;
                string orderQuantity = model.Item1.Quantity.ToString();
                string? paymentAmount = model.Item1.TotalPrice.ToString();

                string pdfFileName = EmailTemplate.InvoicePDFTemplate.GetSubject();
                string pdfBody = EmailTemplate.InvoicePDFTemplate.GetBody(orderDate, paymentDate, recipientName, giftName, orderQuantity, paymentAmount);
                
                string? toEmail = userLogin?.Email;

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, toEmail, pdfBody, pdfFileName);
                HttpContext.Session.Remove("PaymentOrderId");
                return RedirectToAction("MyOrders", "GiftSender");
            }

            return View(model);
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
		[ValidateAntiForgeryToken]
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
                int userLoginId = (int)HttpContext?.Session?.GetInt32("userLoginId");
                var model = _context.UserLogins
                    .Include(x => x.User)
                    .Include(x => x.Role)
                    .Where(x => x.Id == userLoginId)
                    .SingleOrDefault();

                if (model == null)
                    return NotFound();

                return View(model);
            }
            return View();
        }

        public IActionResult EditProfile()
        {
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
        public async Task<IActionResult> EditProfile([Bind("Id,Status,Fname,Lname,ImagePath,ImageFile")] Userr user)
        {
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
                        Console.WriteLine(user.ImagePath);
                        // save path to model
                        user.ImagePath = fileName;
                        Console.WriteLine(user.ImagePath);
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(MyProfile));
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
            await Console.Out.WriteLineAsync("error messages(" + errorMessages.Count + ")=");

            HttpContext.Session.Set("ErrorMessages", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(errorMessages)));

            if (ModelState.IsValid)
            {
                string username = HttpContext.Session.GetString("Username");
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

            return RedirectToAction("EditProfile", "GiftSender");
        }
    }
}
