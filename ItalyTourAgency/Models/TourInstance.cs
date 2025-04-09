using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class TourInstance
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int BookedSlots { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Tour Tour { get; set; } = null!;
}
