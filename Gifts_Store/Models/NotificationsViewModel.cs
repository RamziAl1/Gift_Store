namespace Gifts_Store.Models
{
    public class NotificationsViewModel
    {
        public int Count { get; set; }
        // <body, [Controller, Action]>
        public List<Pair<string, string[]>>? Notifications { get; set; }

        public NotificationsViewModel()
        {
            Count = 0;
            Notifications = new List<Pair<string, string[]>>();
        }

        public NotificationsViewModel(int count, List<Pair<string, string[]>>? notifications)
        {
            Count = count;
            Notifications = notifications;
        }
    }

    public struct Pair<TFirst, TSecond>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }

        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }
    }
}
