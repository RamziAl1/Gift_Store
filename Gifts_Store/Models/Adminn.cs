using System;
using System.Collections.Generic;

namespace Gifts_Store.Models;

public partial class Adminn
{
    public decimal Id { get; set; }

    public decimal? Profit { get; set; }

    public decimal? UserId { get; set; }

    public virtual ICollection<AboutU> AboutUs { get; set; } = new List<AboutU>();

    public virtual ICollection<ContactU> ContactUs { get; set; } = new List<ContactU>();

    public virtual ICollection<Home> Homes { get; set; } = new List<Home>();

    public virtual ICollection<ContactUsEntry> ContactUsEntries { get; set; } = new List<ContactUsEntry>();

    public virtual Userr? User { get; set; }
}
