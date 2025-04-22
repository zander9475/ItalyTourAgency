using System;
using System.Collections.Generic;

namespace ItalyTourAgency.Models;

public partial class Review
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TourId { get; set; }

    public int Rating { get; set; }

    public string? Description { get; set; }

    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
