using System;
using System.Collections.Generic;

namespace Entity;

public partial class CourtOwner
{
    public int CourtOwnerId { get; set; }

    public int? SubId { get; set; }

    public string? TaxCode { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public int? PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public sbyte IsTwoFactorEnabled { get; set; }

    public string? TwoFactorSecret { get; set; }

    public string? FirebaseUid { get; set; }

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

    public virtual PaymentDetail? PaymentDetail { get; set; }

    public virtual Subscription? Sub { get; set; }
}
