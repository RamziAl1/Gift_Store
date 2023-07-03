using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class Testimonial
{
    public decimal Id { get; set; }

	[Required]
	[StringLength(500, ErrorMessage = "Messege must contain a max of 500 characters")]
	public string? Messege { get; set; }

	[StringLength(10, ErrorMessage = "Status must contain a max of 10 characters")]
	public string? Status { get; set; }

    public decimal? UserId { get; set; }

    public virtual Userr? User { get; set; }
}
