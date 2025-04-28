using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class TourInstance
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int BookedSlots { get; set; } = 0;

    public string Status { get; set; } = "Open";

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ValidateNever]
    public virtual Tour Tour { get; set; } = null!;
}
