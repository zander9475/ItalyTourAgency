using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class TourLocation
{
    public int TourId { get; set; }

    public int LocationId { get; set; }

    public int? OrderInTour { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
