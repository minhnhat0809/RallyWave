using Entity;

namespace NotificationManagement.Repository.Impl;

public class NotificationRepo(RallyWaveContext repositoryContext) : RepositoryBase<Notification>(repositoryContext), INotificationRepo
{
    
}