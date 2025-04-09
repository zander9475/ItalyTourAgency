using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<TourLocation> TourLocations { get; set; } = new List<TourLocation>();
}
