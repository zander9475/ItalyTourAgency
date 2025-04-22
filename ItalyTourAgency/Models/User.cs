using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
