using System;
using System.Collections.Generic;

namespace Gifts_Store.Models;

public partial class Rolee
{
    public decimal Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
