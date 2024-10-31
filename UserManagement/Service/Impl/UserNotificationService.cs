using UserManagement.Repository;

namespace UserManagement.Service.Impl;


public class UserNotificationService : BackgroundService
{
    private readonly ILogger<UserNotificationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    // Define the notification threshold, e.g., 30 minutes before the booking
    private readonly TimeSpan _notificationThreshold = TimeSpan.FromMinutes(3);

    public UserNotificationService(ILogger<UserNotificationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UserNotificationService is starting the timer.");
        _timer = new Timer(CheckForVerifyUser, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async void CheckForVerifyUser(object? state)
    {
        _logger.LogInformation("Checking for upcoming user verifications...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

            var notificationTime = DateTime.Now.Add(_notificationThreshold);

            
            var upcomingUsers = await userRepo.GetUnverifiedUsersOlderThan(notificationTime);

            foreach (var user in upcomingUsers)
            {
                SendNotification(user);
                _logger.LogInformation("Notification sent to user with ID {UserId} at {Time}", user.UserId, DateTimeOffset.Now);
            }
            
        }
    }

    private void SendNotification(Object ob)
    {
        // Implement the actual notification logic here, e.g., send email or push notification
        // Example: _notificationService.NotifyUser(booking.UserId, "Your booking starts soon!");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UserNotificationService is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}