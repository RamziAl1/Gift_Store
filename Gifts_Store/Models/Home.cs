using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Gifts_Store.Models;

public partial class Home
{
    public decimal Id { get; set; }

    public string? LogoPath { get; set; }

    public string? BackgroundPath { get; set; }

    [StringLength(30, ErrorMessage = "Site Name must contain a max of 30 characters")]
    public string SiteName { get; set; } = null!;

    [StringLength(200, ErrorMessage = "WelcomeText must contain a max of 200 characters")]
    public string? WelcomeText { get; set; }

    public decimal? AdminId { get; set; }

    [NotMapped]
    public virtual IFormFile? ImageLogo { get; set; }

    [NotMapped]
    public virtual IFormFile? ImageBackground { get; set; }

    public virtual Adminn? Admin { get; set; }
}