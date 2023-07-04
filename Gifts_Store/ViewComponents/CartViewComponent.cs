using Gifts_Store.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gifts_Store.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ModelContext _context;

        public CartViewComponent(ModelContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            if (_context.Orderrs == null)
            {
                return View("_Orders_not_found");
            }

            var giftSenderId = HttpContext.Session.GetInt32("SenderId");
            var query = from gs in _context.GiftSenders
                        join o in _context.Orderrs on gs.Id equals o.GiftSenderId
                        join g in _context.Gifts on o.GiftId equals g.Id
                        where gs.Id == giftSenderId && o.Status == "in cart"
                        select Tuple.Create(g, o);

            if (query == null)
            {
                return View("_No_Orders_Found");
            }

            var ordersInCart = query.ToList();
            NotificationsViewModel cartItems = new NotificationsViewModel();

            ordersInCart.ForEach(x =>
                cartItems?.Notifications?
                .Add(new Pair<string, string[]>($"{x.Item1.Name} x{x.Item2.Quantity.ToString("F0")}"
                , new string[] { "GiftSender", "MyCart" }))
            );
            cartItems.Count = cartItems.Notifications.Count;

            return View(cartItems);
        }
    }
}
