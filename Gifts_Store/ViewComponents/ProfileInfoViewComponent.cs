using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gifts_Store.ViewComponents
{
    public class ProfileInfoViewComponent : ViewComponent
    {
        private readonly ModelContext _context;

        public ProfileInfoViewComponent(ModelContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            if (_context.Userrs == null)
            {
                return View("_Users_not_found");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Userrs.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return View("_User_not_found");
            }

            return View(user);
        }
    }
}
