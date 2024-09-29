﻿using System;
using System.Collections.Generic;

namespace Entity;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? MatchId { get; set; }

    public int? CourtId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public DateTime CreateAt { get; set; }

    public string? Note { get; set; }

    public sbyte Status { get; set; }

    public virtual Court? Court { get; set; }

    public virtual Match? Match { get; set; }

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    public virtual User? User { get; set; }
}
