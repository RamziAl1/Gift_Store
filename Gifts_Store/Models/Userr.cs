using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gifts_Store.Models;

public partial class Userr
{
    public decimal Id { get; set; }


	[Required(ErrorMessage = "First name is required")]
	[StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
	public string Fname { get; set; } = null!;

	[Required(ErrorMessage = "Last name is required")]
	[StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 20 characters")]
	public string Lname { get; set; } = null!;

    public string? ImagePath { get; set; }

    public string? Status { get; set; }

	[NotMapped]
	public virtual IFormFile? ImageFile { get; set; }

	public virtual ICollection<Adminn> Adminns { get; set; } = new List<Adminn>();

    public virtual ICollection<GiftMaker> GiftMakers { get; set; } = new List<GiftMaker>();

    public virtual ICollection<GiftSender> GiftSenders { get; set; } = new List<GiftSender>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
