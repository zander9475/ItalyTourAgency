using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TourId { get; set; }

    public int Rating { get; set; }

    public string? Description { get; set; }

    public DateTime ReviewDate { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
