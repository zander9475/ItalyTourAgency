using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ItalyTourAgency.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TourInstanceId { get; set; }

    public int TourId { get; set; }

    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Group Size")]
    public int GroupSize { get; set; }

    [BindNever]
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }

    [BindNever]
    public DateTime? PaymentDate { get; set; }

    [BindNever]
    [Display(Name = "Booking Status")]
    public string Status { get; set; } = "Pending";

    [BindNever]
    [Display(Name = "Payment Status")]
    public bool PaymentProcessed { get; set; } = false;

    [ValidateNever]
    public virtual TourInstance TourInstance { get; set; } = null!;

    [ValidateNever]
    public virtual Tour Tour { get; set; } = null!;

    [ValidateNever]
    public virtual User User { get; set; } = null!;
}
