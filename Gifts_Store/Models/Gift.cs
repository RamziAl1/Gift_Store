
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gifts_Store.Models;

public partial class Gift
{
    public decimal Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0, 100, ErrorMessage = "Value must be between 0 and 100")]
    public decimal? Sale { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public decimal? Quantity { get; set; }

    public string? ImagePath { get; set; }

    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }


    public DateTime AddedDate { get; set; }

    public decimal? CategoryId { get; set; }

    public decimal? GiftMakerId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual GiftMaker? GiftMaker { get; set; }

    public virtual ICollection<Orderr> Orderrs { get; set; } = new List<Orderr>();
}