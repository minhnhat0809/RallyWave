using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserManagement.Repository;

namespace UserManagement.Service.Impl;

    public class UserNotificationService : IHostedService, IDisposable
    {
        private readonly ILogger<UserNotificationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public UserNotificationService(ILogger<UserNotificationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserNotificationService is starting.");
            // Set the timer to run every 1 minutes
            _timer = new Timer(CheckForExpiredUnverifiedUsers, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void CheckForExpiredUnverifiedUsers(object state)
        {
            _logger.LogInformation("Checking for expired unverified accounts at: {Time}", DateTime.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

                // Fetch unverified users older than one day
                var expiredUnverifiedUsers = await userRepo.GetUnverifiedUsersOlderThanOneDay();

                foreach (var user in expiredUnverifiedUsers)
                {
                    _logger.LogInformation("Deleting unverified user with ID {UserId}, created at {CreatedDate}", user.UserId, user.CreatedDate);
                    // Uncomment below to delete
                    // await userRepo.DeleteUser(user);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserNotificationService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
