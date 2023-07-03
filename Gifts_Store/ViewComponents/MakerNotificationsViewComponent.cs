using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gifts_Store.ViewComponents
{
    public class MakerNotificationsViewComponent : ViewComponent
    {
        private readonly ModelContext _context;

        public MakerNotificationsViewComponent(ModelContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            if (_context.Orderrs == null)
            {
                return View("_Orders_not_found");
            }

            var giftMakerId = HttpContext.Session.GetInt32("MakerId");
            var query = from gm in _context.GiftMakers
                        join g in _context.Gifts on gm.Id equals g.GiftMakerId
                        join o in _context.Orderrs on g.Id equals o.GiftId
                        where gm.Id == giftMakerId && o.Status == "pending"
                        select g;

            if (query == null)
            {
                return View("_No_Pending_Users_Found");
            }

            var pendingGifts = query.AsEnumerable().ToList();
            NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

            pendingGifts.ForEach(x =>
                notificationsViewModel?.Notifications?
                .Add(new Pair<string, string[]>($"New gift request for {x.Name} needs approval"
                , new string[] { "GiftMaker", "MyOrders" }))
            );
            notificationsViewModel.Count = notificationsViewModel.Notifications.Count;

            return View(notificationsViewModel);
        }
    }
}
