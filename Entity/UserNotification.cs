using System;
using System.Collections.Generic;

namespace Entity;

public partial class UserNotification
{
    public int RecipientId { get; set; }

    public int NotificationId { get; set; }

    public DateTime? RecieveAt { get; set; }

    public ulong? IsRead { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User Recipient { get; set; } = null!;
}
