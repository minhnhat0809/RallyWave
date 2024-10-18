using System;
using System.Collections.Generic;

namespace Entity;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public int PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public sbyte IsTwoFactorEnabled { get; set; }

    public string? TwoFactorSecret { get; set; }

    public string? FirebaseUid { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Friendship> FriendshipUser1s { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUser2s { get; set; } = new List<Friendship>();

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<UserMatch> UserMatches { get; set; } = new List<UserMatch>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

    public virtual ICollection<Conservation> Conservations { get; set; } = new List<Conservation>();
}
