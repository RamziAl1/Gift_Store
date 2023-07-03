using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class UserLogin
{
    public decimal Id { get; set; }

	[Required(ErrorMessage = "Username is required")]
	[StringLength(200, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
	public string UserName { get; set; } = null!;

	[StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters and less than 50")]
	[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
			ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
	[Required(ErrorMessage = "Password is required")]
	public string Password { get; set; } = null!;

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Invalid email address")]
	[StringLength(50, MinimumLength = 8, ErrorMessage = "Email must be at least 8 characters and less than 50")]
	public string Email { get; set; } = null!;

    public decimal? RoleId { get; set; }

    public decimal? UserId { get; set; }

    public virtual Rolee? Role { get; set; }

    public virtual Userr? User { get; set; }
}
