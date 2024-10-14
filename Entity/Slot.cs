using System;
using System.Collections.Generic;

namespace Entity;

public partial class Slot
{
    public int SlotId { get; set; }

    public int? CourtId { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public double Cost { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Court? Court { get; set; }
}
