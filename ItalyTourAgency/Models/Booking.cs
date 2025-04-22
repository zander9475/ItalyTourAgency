using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TourInstanceId { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    public int GroupSize { get; set; }

    public decimal TotalPrice { get; set; }

    public string? CardNumber { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string Status { get; set; } = "Pending";

    public bool PaymentProcessed { get; set; } = false;

    public virtual TourInstance TourInstance { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
