using System;
using System.Collections.Generic;

namespace Entity;

public partial class CourtOwner
{
    public int CourtOwnerId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public int? PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();
}
