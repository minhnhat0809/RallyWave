using System;
using System.Collections.Generic;

namespace Entity;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Category { get; set; }

    public DateTime CreateAt { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
