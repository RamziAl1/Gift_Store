using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class Category
{
    public decimal Id { get; set; }

	[Required(ErrorMessage = "CategoryName is required")]
	[StringLength(50, ErrorMessage = "CategoryName must contain a max of 50 characters")]
	public string CategoryName { get; set; } = null!;

    public virtual ICollection<GiftMaker> GiftMakers { get; set; } = new List<GiftMaker>();

    public virtual ICollection<Gift> Gifts { get; set; } = new List<Gift>();
}
