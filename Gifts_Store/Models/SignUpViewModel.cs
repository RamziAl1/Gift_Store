using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models
{
	public class SignUpViewModel
	{
		[Required(ErrorMessage = "First name is required")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
		public string Fname { get; set; } = null!;

		[Required(ErrorMessage = "Last name is required")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 20 characters")]
		public string Lname { get; set; } = null!;

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

		public virtual IFormFile? ImageFile { get; set; }
	}
}
