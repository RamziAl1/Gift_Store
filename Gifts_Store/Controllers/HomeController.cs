using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Gifts_Store.Controllers
{
	public class HomeController : Controller
	{
		private readonly ModelContext _context;

		public HomeController(ModelContext context)
		{
			_context = context;
		}
		public IActionResult Home()
		{
			if (_context.Homes == null
				|| _context.AboutUs == null
				|| _context.ContactUs == null
				|| _context.Testimonials == null
					)
			{
				return NotFound();
			}

			decimal id = 1;

			var homePage = _context.Homes.Find(id);
			var aboutUs = _context.AboutUs.Find(id);
			var contactUs = _context.ContactUs.Find(id);
			var testimonials = _context.Testimonials.Include(p => p.User).Where(x => x.Status == "approved").ToList();

            HttpContext.Session.SetString("homePage", JsonConvert.SerializeObject(homePage));
            HttpContext.Session.SetString("homePage", JsonConvert.SerializeObject(homePage));

            if (homePage == null || aboutUs == null || contactUs == null || testimonials == null)
			{
				return NotFound();
			}

			var model = Tuple.Create<Home, AboutU, ContactU, IEnumerable<Testimonial>>(homePage, aboutUs, contactUs, testimonials);

			return View(model);
		}

		public IActionResult Testimonials()
		{
			if (_context.Testimonials == null)
			{
				return NotFound();
			}

			var testimonials = _context.Testimonials.Include(p => p.User).Where(x => x.Status == "approved").ToList();

			if (testimonials == null)
			{
				return NotFound();
			}

			return View(testimonials);
		}

		public IActionResult ContactUs()
		{
			if (_context.ContactUs == null)
			{
				return NotFound();
			}

			decimal id = 1;

			var contactUs = _context.ContactUs.Find(id);

			if (contactUs == null)
			{
				return NotFound();
			}

			return View(contactUs);
		}

		public IActionResult AboutUs()
		{
			if (_context.AboutUs == null)
			{
				return NotFound();
			}

			decimal id = 1;

			var aboutUs = _context.AboutUs.Find(id);

			if (aboutUs == null)
			{
				return NotFound();
			}

			return View(aboutUs);
		}

        [HttpPost]
        public async Task<IActionResult> AddContactUsEntry(string name, string email, string subject, string message)
        {
			ContactUsEntry contactUsEntry = new ContactUsEntry();
			contactUsEntry.Name = name;
			contactUsEntry.Email = email;
			contactUsEntry.Subject = subject;
			contactUsEntry.Message = message;
			contactUsEntry.AdminId = 1;

            _context.Add(contactUsEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Home));
        }
    }
}
