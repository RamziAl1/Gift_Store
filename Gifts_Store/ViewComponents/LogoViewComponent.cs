using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gifts_Store.ViewComponents
{
	public class LogoViewComponent: ViewComponent
	{
		private readonly ModelContext _context;

		public LogoViewComponent(ModelContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke()
		{
			if (_context.Homes == null)
			{
				return View("_Homes_not_found");
			}

			decimal id = 1;

			var homePage = _context.Homes.Find(id);
			if (homePage == null)
			{
				return View("_Homepage_with_id=1_not_found");
			}

			return View(homePage);
		}
	}

	
}
