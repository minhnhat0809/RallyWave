using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Entity;

public partial class RallywaveContext : DbContext
{
    public RallywaveContext()
    {
    }

    public RallywaveContext(DbContextOptions<RallywaveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Conservation> Conservations { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<CourtImage> CourtImages { get; set; }

    public virtual DbSet<CourtOwner> CourtOwners { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserMatch> UserMatches { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UserSport> UserSports { get; set; }

    public virtual DbSet<UserTeam> UserTeams { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("booking");

            entity.HasIndex(e => e.UserId, "FK_Booking_User");

            entity.HasIndex(e => new { e.CourtId, e.Date, e.TimeStart, e.TimeEnd }, "idx_booking_court_date_time");

            entity.HasIndex(e => e.MatchId, "match_id").IsUnique();

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.CourtId).HasColumnName("court_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TimeEnd)
                .HasColumnType("time")
                .HasColumnName("time_end");
            entity.Property(e => e.TimeStart)
                .HasColumnType("time")
                .HasColumnName("time_start");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Court).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Court");

            entity.HasOne(d => d.Match).WithOne(p => p.Booking)
                .HasForeignKey<Booking>(d => d.MatchId)
                .HasConstraintName("FK_Booking_Match");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<Conservation>(entity =>
        {
            entity.HasKey(e => e.ConservationId).HasName("PRIMARY");

            entity.ToTable("conservation");

            entity.HasIndex(e => e.MatchId, "match_id").IsUnique();

            entity.HasIndex(e => e.TeamId, "team_id").IsUnique();

            entity.Property(e => e.ConservationId).HasColumnName("conservation_id");
            entity.Property(e => e.ConservationName)
                .HasMaxLength(50)
                .HasColumnName("conservation_name");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TeamId).HasColumnName("team_id");

            entity.HasOne(d => d.Match).WithOne(p => p.Conservation)
                .HasForeignKey<Conservation>(d => d.MatchId)
                .HasConstraintName("FK_Conservation_Match");

            entity.HasOne(d => d.Team).WithOne(p => p.Conservation)
                .HasForeignKey<Conservation>(d => d.TeamId)
                .HasConstraintName("FK_Conservation_Team");
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId).HasName("PRIMARY");

            entity.ToTable("court");

            entity.HasIndex(e => e.CourtOwnerId, "FK_Court_Owner");

            entity.HasIndex(e => e.SportId, "FK_Court_Sport");

            entity.Property(e => e.CourtId).HasColumnName("court_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CourtName)
                .HasMaxLength(100)
                .HasColumnName("court_name");
            entity.Property(e => e.CourtOwnerId).HasColumnName("court_owner_id");
            entity.Property(e => e.MaxPlayers).HasColumnName("max_players");
            entity.Property(e => e.Province)
                .HasMaxLength(255)
                .HasColumnName("province");
            entity.Property(e => e.SportId).HasColumnName("sport_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.CourtOwner).WithMany(p => p.Courts)
                .HasForeignKey(d => d.CourtOwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Court_Owner");

            entity.HasOne(d => d.Sport).WithMany(p => p.Courts)
                .HasForeignKey(d => d.SportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Court_Sport");
        });

        modelBuilder.Entity<CourtImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.ToTable("court_image");

            entity.HasIndex(e => e.CourtId, "FK_Court_Image_Court");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.CourtId).HasColumnName("court_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(2083)
                .HasColumnName("image_url");

            entity.HasOne(d => d.Court).WithMany(p => p.CourtImages)
                .HasForeignKey(d => d.CourtId)
                .HasConstraintName("FK_Court_Image_Court");
        });

        modelBuilder.Entity<CourtOwner>(entity =>
        {
            entity.HasKey(e => e.CourtOwnerId).HasName("PRIMARY");

            entity.ToTable("court_owner");

            entity.HasIndex(e => e.SubId, "FK_Court_Owner_Subscription");

            entity.Property(e => e.CourtOwnerId).HasColumnName("court_owner_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirebaseUid)
                .HasMaxLength(255)
                .HasColumnName("firebase_uid");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.IsTwoFactorEnabled).HasColumnName("is_two_factor_enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .HasColumnName("password_salt");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Province)
                .HasMaxLength(255)
                .HasColumnName("province");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubId).HasColumnName("sub_id");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(15)
                .HasColumnName("tax_code");
            entity.Property(e => e.TwoFactorSecret)
                .HasMaxLength(255)
                .HasColumnName("two_factor_secret");

            entity.HasOne(d => d.Sub).WithMany(p => p.CourtOwners)
                .HasForeignKey(d => d.SubId)
                .HasConstraintName("FK_Court_Owner_Subscription");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => new { e.User1Id, e.User2Id })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("friendship");

            entity.HasIndex(e => e.User2Id, "FK_Friend_Ship_User2");

            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");
            entity.Property(e => e.Level).HasColumnName("level");

            entity.HasOne(d => d.User1).WithMany(p => p.FriendshipUser1s)
                .HasForeignKey(d => d.User1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_Ship_User1");

            entity.HasOne(d => d.User2).WithMany(p => p.FriendshipUser2s)
                .HasForeignKey(d => d.User2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friend_Ship_User2");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PRIMARY");

            entity.ToTable("match");

            entity.HasIndex(e => e.SportId, "FK_Match_Sport");

            entity.HasIndex(e => e.CreateBy, "FK_Match_User");

            entity.HasIndex(e => new { e.Date, e.TimeStart, e.TimeEnd }, "IX_Match_Date_TimeStart_TimeEnd");

            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.AddByOthers)
                .HasColumnType("bit(1)")
                .HasColumnName("add_by_others");
            entity.Property(e => e.AutoApprove)
                .HasColumnType("bit(1)")
                .HasColumnName("auto_approve");
            entity.Property(e => e.BlockingOff).HasColumnName("blocking_off");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Iteration).HasColumnName("iteration");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MatchName)
                .HasMaxLength(100)
                .HasColumnName("match_name");
            entity.Property(e => e.MatchType).HasColumnName("match_type");
            entity.Property(e => e.MaxAge).HasColumnName("max_age");
            entity.Property(e => e.MaxLevel).HasColumnName("max_level");
            entity.Property(e => e.MinAge).HasColumnName("min_age");
            entity.Property(e => e.MinLevel).HasColumnName("min_level");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Notification)
                .HasColumnType("bit(1)")
                .HasColumnName("notification");
            entity.Property(e => e.SportId).HasColumnName("sport_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TeamSize).HasColumnName("team_size");
            entity.Property(e => e.TimeEnd)
                .HasColumnType("time")
                .HasColumnName("time_end");
            entity.Property(e => e.TimeStart)
                .HasColumnType("time")
                .HasColumnName("time_start");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_User");

            entity.HasOne(d => d.Sport).WithMany(p => p.Matches)
                .HasForeignKey(d => d.SportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Sport");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("message");

            entity.HasIndex(e => e.ConservationId, "FK_Message_Conservation");

            entity.HasIndex(e => e.Sender, "FK_Message_Sender");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ConservationId).HasColumnName("conservation_id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.DateTime)
                .HasColumnType("datetime")
                .HasColumnName("date_time");
            entity.Property(e => e.Sender).HasColumnName("sender");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Conservation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConservationId)
                .HasConstraintName("FK_Message_Conservation");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Sender");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity.ToTable("notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Category)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("category");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payment_detail");

            entity.HasIndex(e => e.BookingId, "booking_id").IsUnique();

            entity.HasIndex(e => e.SubId, "sub_id").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubId).HasColumnName("sub_id");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("type");

            entity.HasOne(d => d.Booking).WithOne(p => p.PaymentDetail)
                .HasForeignKey<PaymentDetail>(d => d.BookingId)
                .HasConstraintName("FK_Payment_Booking");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PRIMARY");

            entity.ToTable("slot");

            entity.HasIndex(e => e.CourtId, "FK_Slot_Court");

            entity.Property(e => e.SlotId).HasColumnName("slot_id");
            entity.Property(e => e.Cost).HasColumnName("cost");
            entity.Property(e => e.CourtId).HasColumnName("court_id");
            entity.Property(e => e.TimeEnd)
                .HasColumnType("time")
                .HasColumnName("time_end");
            entity.Property(e => e.TimeStart)
                .HasColumnType("time")
                .HasColumnName("time_start");

            entity.HasOne(d => d.Court).WithMany(p => p.Slots)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slot_Court");
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.SportId).HasName("PRIMARY");

            entity.ToTable("sport");

            entity.Property(e => e.SportId).HasColumnName("sport_id");
            entity.Property(e => e.SportName)
                .HasMaxLength(30)
                .HasColumnName("sport_name");
            entity.Property(e => e.Status)
                .HasColumnType("bit(1)")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubId).HasName("PRIMARY");

            entity.ToTable("subscription");

            entity.Property(e => e.SubId).HasColumnName("sub_id");
            entity.Property(e => e.IsActive)
                .HasColumnType("bit(1)")
                .HasColumnName("isActive");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.SubDescription)
                .HasMaxLength(255)
                .HasColumnName("sub_description");
            entity.Property(e => e.SubName)
                .HasMaxLength(100)
                .HasColumnName("sub_name");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PRIMARY");

            entity.ToTable("team");

            entity.HasIndex(e => e.SportId, "FK_Team_Sport");

            entity.HasIndex(e => e.CreateBy, "FK_Team_User");

            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.SportId).HasColumnName("sport_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .HasColumnName("team_name");
            entity.Property(e => e.TeamSize).HasColumnName("team_size");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Teams)
                .HasForeignKey(d => d.CreateBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_User");

            entity.HasOne(d => d.Sport).WithMany(p => p.Teams)
                .HasForeignKey(d => d.SportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_Sport");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.SubId, "FK_User_Subscription");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirebaseUid)
                .HasMaxLength(255)
                .HasColumnName("firebase_uid");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.IsTwoFactorEnabled).HasColumnName("is_two_factor_enabled");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .HasColumnName("password_salt");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Province)
                .HasMaxLength(255)
                .HasColumnName("province");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SubId).HasColumnName("sub_id");
            entity.Property(e => e.TwoFactorSecret)
                .HasMaxLength(255)
                .HasColumnName("two_factor_secret");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Sub).WithMany(p => p.Users)
                .HasForeignKey(d => d.SubId)
                .HasConstraintName("FK_User_Subscription");

            entity.HasMany(d => d.Conservations).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserConservation",
                    r => r.HasOne<Conservation>().WithMany()
                        .HasForeignKey("ConservationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Conservation_Conservation"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Conservation_User"),
                    j =>
                    {
                        j.HasKey("UserId", "ConservationId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("user_conservation");
                        j.HasIndex(new[] { "ConservationId" }, "FK_User_Conservation_Conservation");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("ConservationId").HasColumnName("conservation_id");
                    });
        });

        modelBuilder.Entity<UserMatch>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MatchId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("user_match");

            entity.HasIndex(e => e.MatchId, "FK_User_Match_Match");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Match).WithMany(p => p.UserMatches)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Match_Match");

            entity.HasOne(d => d.User).WithMany(p => p.UserMatches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Match_User");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => new { e.RecipientId, e.NotificationId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("user_notification");

            entity.HasIndex(e => e.NotificationId, "FK_User_Notification_Notification");

            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.IsRead)
                .HasColumnType("bit(1)")
                .HasColumnName("isRead");
            entity.Property(e => e.RecieveAt)
                .HasColumnType("datetime")
                .HasColumnName("recieve_at");

            entity.HasOne(d => d.Notification).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Notification_Notification");

            entity.HasOne(d => d.Recipient).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Notification_User");
        });

        modelBuilder.Entity<UserSport>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SportId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("user_sport");

            entity.HasIndex(e => e.SportId, "FK_User_Sport_Sport");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SportId).HasColumnName("sport_id");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Status)
                .HasColumnType("bit(1)")
                .HasColumnName("status");

            entity.HasOne(d => d.Sport).WithMany(p => p.UserSports)
                .HasForeignKey(d => d.SportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Sport_Sport");

            entity.HasOne(d => d.User).WithMany(p => p.UserSports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Sport_User");
        });

        modelBuilder.Entity<UserTeam>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TeamId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("user_team");

            entity.HasIndex(e => e.TeamId, "FK_User_Team_Team");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.Status)
                .HasColumnType("bit(1)")
                .HasColumnName("status");

            entity.HasOne(d => d.Team).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Team_Team");

            entity.HasOne(d => d.User).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Team_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
