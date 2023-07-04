using System;
using System.Collections.Generic;

namespace Gifts_Store.Models;

public partial class GiftSender
{
    public decimal Id { get; set; }

    public decimal? UserId { get; set; }

    public virtual ICollection<Orderr> Orderrs { get; set; } = new List<Orderr>();

    public virtual Userr? User { get; set; }
}