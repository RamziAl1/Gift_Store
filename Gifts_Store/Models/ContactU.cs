using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class ContactU
{
    public decimal Id { get; set; }

    [Required(ErrorMessage = "MobileNumber is required")]
    [RegularExpression(@"^\+[0-9]{12}$", ErrorMessage = "Please enter a valid number starting with '+' and having 13 characters.")]
    public string MobileNumber { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "PoBox is required")]
    [StringLength(30, ErrorMessage = "PO Box must contain a max of 30 characters")]
    public string PoBox { get; set; } = null!;

    public decimal? AdminId { get; set; }

    public virtual Adminn? Admin { get; set; }
}