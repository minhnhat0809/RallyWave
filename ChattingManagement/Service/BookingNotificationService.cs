namespace ChattingManagement.Service;

public class BookingNotificationService : BackgroundService
{
    private readonly ILogger<BookingNotificationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    // Define the notification threshold, e.g., 30 minutes before the booking
    private readonly TimeSpan _notificationThreshold = TimeSpan.FromMinutes(30);

    public BookingNotificationService(ILogger<BookingNotificationService> logger, IServiceProvider serviceProvider, Timer timer)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _timer = timer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Check every 5 minutes for upcoming bookings
        _timer = new Timer(CheckForUpcomingBookings, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private void CheckForUpcomingBookings(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            // Get your booking service, which retrieves bookings from the database
            //var bookingService = scope.ServiceProvider.GetRequiredService<I>();

            // Calculate the range of times to check for bookings
            //var notificationTime = DateTime.Now.Add(NotificationThreshold);
            
            // Get all bookings that are within the notification threshold and have not been notified
            //var upcomingBookings = bookingService.GetUpcomingBookings(notificationTime);

            /*foreach (var booking in upcomingBookings)
            {
                // Send notification to user about the upcoming booking
                SendNotification(booking);
                
                _logger.LogInformation("Notification sent for booking {BookingId} at {Time}", booking.Id, DateTimeOffset.Now);

                // Mark this booking as notified to prevent duplicate notifications
                bookingService.MarkAsNotified(booking.Id);
            }*/
        }
    }

    private void SendNotification(Object ob)
    {
        // Implement the actual notification logic here, e.g., send email or push notification
        // Example: _notificationService.NotifyUser(booking.UserId, "Your booking starts soon!");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BookingNotificationService is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}