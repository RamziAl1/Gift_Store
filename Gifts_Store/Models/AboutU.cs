using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class AboutU
{
    public decimal Id { get; set; }

	[Required(ErrorMessage = "Introtext is required")]
	[StringLength(200, ErrorMessage ="IntroText must contain a max of 200 characters")]
    public string? IntroText { get; set; }

    public decimal? AdminId { get; set; }

    public virtual Adminn? Admin { get; set; }
}
