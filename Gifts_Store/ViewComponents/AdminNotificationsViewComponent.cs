using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gifts_Store.ViewComponents
{
    public class AdminNotificationsViewComponent : ViewComponent
    {
        private readonly ModelContext _context;

        public AdminNotificationsViewComponent(ModelContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            if (_context.Userrs == null)
            {
                return View("_Users_not_found");
            }

            var makerQuery = from u in _context.Userrs
                        where u.Status == "pending"
                        select u;

            if (makerQuery == null)
            {
                return View("_No_Pending_Users_Found");
            }

            var pendingGiftMakers = makerQuery.ToList();
            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();
            
            pendingGiftMakers.ForEach(x => 
                notificationsViewModel?.Notifications?
                .Add(new Pair<string, string[]>($"New gift maker {x.Fname} {x.Lname}'s account needs approval"
                , new string[] {"Admin", "PendingGiftMakers" }))
            );

            //var testimonialQuery = from u in _context.Testimonials
            //            where u.Status == "pending"
            //            select u;
            var testimonialQuery = _context.Testimonials
                .Include(x => x.User)
                .Where(x => x.Status == "pending");

            if (testimonialQuery == null)
            {
                return View("_No_Pending_Testimonials_Found");
            }

            var pendingTestimonials = testimonialQuery.ToList();

            pendingTestimonials.ForEach(x =>
                notificationsViewModel?.Notifications?
                .Add(new Pair<string, string[]>($"New Testimonial from {x.User?.Fname} {x.User?.Lname} needs approval"
                , new string[] { "Testimonials", "Index" }))
            );
            notificationsViewModel.Count += notificationsViewModel.Notifications.Count;

            return View(notificationsViewModel);
        }
    }
}
