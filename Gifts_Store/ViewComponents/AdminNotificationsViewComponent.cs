using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;

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

            var query = from u in _context.Userrs
                        where u.Status == "pending"
                        select u;

            if (query == null)
            {
                return View("_No_Pending_Users_Found");
            }

            var pendingGiftMakers = query.AsEnumerable().ToList();
            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();
            
            pendingGiftMakers.ForEach(x => 
                notificationsViewModel?.Notifications?
                .Add(new Pair<string, string[]>($"New gift maker {x.Fname} {x.Lname}'s account needs approval"
                , new string[] {"Admin", "PendingGiftMakers" }))
            );
            notificationsViewModel.Count = notificationsViewModel.Notifications.Count;

            return View(notificationsViewModel);
        }
    }
}
