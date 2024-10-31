using System;
using System.Collections.Generic;

namespace Entity;

public partial class Friendship
{
    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int? Level { get; set; }

    public sbyte? Status { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
