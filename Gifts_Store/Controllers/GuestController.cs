using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gifts_Store.Controllers
{
	public class GuestController : Controller
	{
		private readonly ModelContext _context;

		public GuestController(ModelContext context, IWebHostEnvironment environment)
		{
			_context = context;
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
			else if ((giftName == null || giftName == "") && giftCategory != "All")
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
	}
}
