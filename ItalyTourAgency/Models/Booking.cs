using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TourInstanceId { get; set; }

    public int TourId { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    public int GroupSize { get; set; }

    [BindNever]
    public decimal TotalPrice { get; set; }

    [BindNever]
    public DateTime? PaymentDate { get; set; }

    [BindNever]
    public string Status { get; set; } = "Pending";

    [BindNever]
    public bool PaymentProcessed { get; set; } = false;

    [ValidateNever]
    public virtual TourInstance TourInstance { get; set; } = null!;

    [ValidateNever]
    public virtual Tour Tour { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
