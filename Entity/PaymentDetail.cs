using System;
using System.Collections.Generic;

namespace Entity;

public partial class PaymentDetail
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public string? Note { get; set; }

    public double Total { get; set; }

    public string Type { get; set; } = null!;

    public sbyte Status { get; set; }

    public virtual Booking? Booking { get; set; }
}
