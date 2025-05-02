using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItalyTourAgency.Models;

public partial class Tour
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Duration { get; set; }

    public decimal? Price { get; set; }

    public int? Capacity { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<TourInstance> TourInstances { get; set; } = new List<TourInstance>();

    public virtual ICollection<TourLocation> TourLocations { get; set; } = new List<TourLocation>();
}
