using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models
{
    public partial class ContactUsEntry
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must contain a max of 50 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, ErrorMessage = "Email must contain a max of 50 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(20, ErrorMessage = "Subject must contain a max of 20 characters")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(500, ErrorMessage = "Message must contain a max of 500 characters")]
        public string Message { get; set; } = null!;
        public decimal? AdminId { get; set; }

        public virtual Adminn? Admin { get; set; }
    }
}