using System;
using System.Collections.Generic;

namespace Gifts_Store.Models;

public partial class GiftMaker
{
    public decimal Id { get; set; }

    public decimal? Profit { get; set; }

    public decimal? UserId { get; set; }

    public decimal? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Gift> Gifts { get; set; } = new List<Gift>();

    public virtual Userr? User { get; set; }
}
