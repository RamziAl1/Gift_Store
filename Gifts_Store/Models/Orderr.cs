using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gifts_Store.Models;

public partial class Orderr
{
    public decimal Id { get; set; }

    
    public DateTime OrderDate { get; set; }

	public DateTime ExpectedArrivalDate { get; set; }

	[Required]
    [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
    public string Address { get; set; } = null!;

	[Required]
    [Range(0.01, Double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public decimal Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public bool? HasArrived { get; set; }

    public bool? PaymentMade { get; set; }

    public decimal? GiftSenderId { get; set; }

    public decimal? GiftId { get; set; }

    public decimal? VisaInfoId { get; set; }

    public virtual Gift? Gift { get; set; }

    public virtual GiftSender? GiftSender { get; set; }

    public virtual VisaInfo? VisaInfo { get; set; }
}
