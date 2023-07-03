using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class VisaInfo
{
    public decimal Id { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Card holder name must be 50 characters or less")]
    public string CardHolderName { get; set; } = null!;

    [Required]
    [StringLength(10, MinimumLength =10, ErrorMessage = "Card number must be 10 digits")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
    public string CardNumber { get; set; } = null!;

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "CCV number must be 3 digits")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
    public string Ccv { get; set; } = null!;

    [Required]
    public DateTime ExpireDate { get; set; }

    public decimal? Balance { get; set; }
}
