using Gifts_Store.Constants;
using Gifts_Store.Enums;
using Gifts_Store.Models;
using Gifts_Store.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text;

namespace Gifts_Store.Controllers
{
    public class AdminController : Controller
    {
		private readonly ModelContext _context;
		private readonly IWebHostEnvironment _environment;

        public AdminController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

		public IActionResult DashBoard()
        {
            var orders = _context.Orderrs.ToList();
            Dictionary<string, int> statistics = new Dictionary<string, int>
            {
                { "approvedOrderCount", orders.Where(x => x.Status == "approved").Count() },
                { "totalOrderCount", orders.Where(x => x.Status != "in cart").Count() },
                { "giftCount", _context.Gifts.Count() },
                { "userCount", _context.Userrs.Count() },
                { "categoryCount", _context.Categories.Count() }
            };

            return View(statistics);
        }

        [HttpGet]
        public List<object> GetFinancialReport()
        {
            List<object> data = new List<object>();
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year + 1, 1, 1);

            var payedOrders = _context.Orderrs
                .Where(x => x.PaymentMade == true).ToList();

            if (payedOrders == null)
                return data;

            var labelsAndProfits = GetMonthlyProfitsInInterval(payedOrders, startDate, endDate);

            data.Add(labelsAndProfits.Item1);
            data.Add(labelsAndProfits.Item2);

            return data;
        }

        [HttpPost]
        public List<object>? GetFinancialReport(string periodType, string startYear, string startMonth, string endYear, string endMonth)
        {
            List<object> data = new List<object>();
            DateTime startDate = new DateTime(Int32.Parse(startYear), Int32.Parse(startMonth), 1);
            DateTime endDate = new DateTime(Int32.Parse(endYear), Int32.Parse(endMonth), 1);

            var payedOrders = _context.Orderrs
                .Where(x => x.PaymentMade == true).ToList();

            if (payedOrders == null)
                return data;

            Tuple<List<string>, List<decimal?>> labelsAndProfits;

            if (periodType == "Month")
                labelsAndProfits = GetMonthlyProfitsInInterval(payedOrders, startDate, endDate);
            else if (periodType == "Year")
            {
                labelsAndProfits = GetYearlyProfitsInInterval(payedOrders, startDate, endDate);
            }
            else
            {
                return null;
            }

            data.Add(labelsAndProfits.Item1);
            data.Add(labelsAndProfits.Item2);

            return data;
        }

        public IActionResult GiftsWithCategories()
        {
            var giftsWithCategories = (from c in _context.Categories
                        join g in _context.Gifts on c.Id equals g.CategoryId
                        join gm in _context.GiftMakers on g.GiftMakerId equals gm.Id
                        join u in _context.Userrs on gm.UserId equals u.Id
                        join ul in _context.UserLogins on u.Id equals ul.UserId
                        select Tuple.Create(g, ul.UserName, c.CategoryName))
                        .ToList();

            return View(giftsWithCategories);
        }

        public IActionResult PendingGiftMakers()
        {
            var pendingGiftMakers = (from u in _context.Userrs
                        join gm in _context.GiftMakers on u.Id equals gm.UserId
                        join c in _context.Categories on gm.CategoryId equals c.Id
                        where u.Status == "pending"
                        select Tuple.Create(u, c))
                        .AsEnumerable();

            return View(pendingGiftMakers);
        }

        [HttpPost]
        public async Task<IActionResult> RejectMaker(decimal? id)
        {
            if (_context.Userrs == null)
            {
                return NotFound("Entity set 'ModelContext.Userrs' is null.");
            }
            var userr = await _context.Userrs.FindAsync(id);

            if (userr == null)
            {
                return NotFound("User with that id not found");
            }

            if (userr.ImagePath != null)
            {
                string wwwRootPath = _environment.WebRootPath;
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

			var userLogin = await _context.UserLogins.SingleOrDefaultAsync(x => x.UserId == userr.Id);

			if (userLogin == null)
			{
				throw new Exception("UserLogin with that id not found");
			}

			Task.Run(() =>
            {
                string email = userLogin.Email;
                string recipientName = $"{userr?.Fname} {userr?.Lname}";
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.MembershipRejection.GetSubject();
                string emailBody = EmailTemplate.MembershipRejection.GetBody(recipientName, senderName);

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, email);
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingGiftMakers));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveMaker(decimal id)
        {
            if (_context.Userrs == null)
            {
                return Problem("Entity set 'ModelContext.Userrs' is null.");
            }
            var userr = await _context.Userrs.FindAsync(id);

            if (userr == null)
            {
                return NotFound("User with that id not found");
            }

            try
            {
                userr.Status = "approved";
                _context.Update(userr);
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

			var userLogin = await _context.UserLogins.SingleOrDefaultAsync(x => x.UserId == userr.Id);
			if (userLogin == null)
			{
				throw new Exception("UserLogin with that id not found");
			}

			Task.Run(() =>
            {
                string recipientName = $"{userr.Fname} {userr.Lname}";
                string username = userLogin.UserName;
                string email = userLogin.Email;
                string senderName = "Gift Store";

                string emailSubject = EmailTemplate.MembershipApproval.GetSubject();
                string emailBody = EmailTemplate.MembershipApproval.GetBody(recipientName, username, email, senderName);

                EmailService.SendEmailFromGiftShop(emailBody, emailSubject, email);
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PendingGiftMakers));
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
                        // save path to model
                        user.ImagePath = fileName;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
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
            return RedirectToAction("EditProfile", "Admin");
        }


        private decimal? GetSingleProfitsInInterval(List<Orderr>? payedOrders, DateTime startDate, DateTime endDate)
        {
            decimal? sumOfProfits = 0;

            var ordersTotalPrices = payedOrders?
                .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate)
                .Select(x => x.TotalPrice)
                .ToList();

            ordersTotalPrices?.ForEach(totalPrice =>
                sumOfProfits += totalPrice * ProfitDistribution.AdminPercentage
            );

            return sumOfProfits;
        }

        private Tuple<List<string>, List<decimal?>> GetMonthlyProfitsInInterval(List<Orderr>? payedOrders, DateTime startDate, DateTime endDate)
        {
            List<decimal?> profits = new List<decimal?>();
            List<DateTime> monthsList = new List<DateTime>();

            DateTime currentMonth = startDate;

            while (currentMonth.Year < endDate.Year || (currentMonth.Year == endDate.Year && currentMonth.Month <= endDate.Month + 1))
            {
                monthsList.Add(currentMonth);

                // Move to the next month
                currentMonth = currentMonth.AddMonths(1);
            }

            for (int i = 0; i < monthsList.Count - 1; i++)
            {
                profits.Add(GetSingleProfitsInInterval(payedOrders, monthsList.ElementAt(i), monthsList.ElementAt(i + 1)));
            }

            List<string> months = new List<string>();
            monthsList.RemoveAt(monthsList.Count - 1);
            foreach (var monthDate in monthsList)
            {
                months.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthDate.Month) + "," + monthDate.Year.ToString());
            }

            return Tuple.Create(months, profits);
        }

        private Tuple<List<string>, List<decimal?>> GetYearlyProfitsInInterval(List<Orderr>? payedOrders, DateTime startDate, DateTime endDate)
        {
            List<decimal?> profits = new List<decimal?>();
            List<DateTime> yearsList = new List<DateTime>();

            DateTime currentYear = startDate;

            while (currentYear.Year <= endDate.Year + 1)
            {
                yearsList.Add(currentYear);

                // Move to the next month
                currentYear = currentYear.AddYears(1);
            }

            for (int i = 0; i < yearsList.Count - 1; i++)
            {
                profits.Add(GetSingleProfitsInInterval(payedOrders, yearsList.ElementAt(i), yearsList.ElementAt(i + 1)));
            }

            List<string> years = new List<string>();
            yearsList.RemoveAt(yearsList.Count - 1);
            foreach (var yearDate in yearsList)
            {
                years.Add(yearDate.Year.ToString());
            }

            return Tuple.Create(years, profits);
        }

        private bool UserrExists(decimal id)
        {
            return (_context.Userrs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
