using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItalyTourAgency.Models;

public partial class TourInstance
{
    public int Id { get; set; }

    public int TourId { get; set; }

    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; }

    [Display(Name = "End Date")]
    public DateOnly EndDate { get; set; }

    [Display(Name = "Max Capacity")]
    public int MaxCapacity { get; set; }

    [Display(Name = "Booked Slots")]
    public int BookedSlots { get; set; } = 0;

    public string Status { get; set; } = null!;

    public void UpdateStatus()
    {
        Status = BookedSlots >= MaxCapacity ? "Full" : "Open";
    }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ValidateNever]
    public virtual Tour Tour { get; set; } = null!;
}
