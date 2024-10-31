using System;
using System.Collections.Generic;

namespace Entity;

public partial class PaymentDetail
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public int? UserId { get; set; }

    public int? CourtOwnerId { get; set; }

    public int? SubId { get; set; }

    public string? Note { get; set; }

    public string Signature { get; set; } = null!;

    public double Total { get; set; }

    public string Type { get; set; } = null!;

    public sbyte Status { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual CourtOwner? CourtOwner { get; set; }

    public virtual Subscription? Sub { get; set; }

    public virtual User? User { get; set; }
}
