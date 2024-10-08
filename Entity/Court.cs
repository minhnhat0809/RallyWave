using System;
using System.Collections.Generic;

namespace Entity;

public partial class Court
{
    public int CourtId { get; set; }

    public int? CourtOwnerId { get; set; }

    public int? SportId { get; set; }

    public string CourtName { get; set; } = null!;

    public sbyte? MaxPlayers { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public sbyte Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CourtImage> CourtImages { get; set; } = new List<CourtImage>();

    public virtual CourtOwner? CourtOwner { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual Sport? Sport { get; set; }
}
