using Entity;

namespace NotificationManagement.Repository.Impl;

public class NotificationRepo(RallywaveContext repositoryContext) : RepositoryBase<Notification>(repositoryContext), INotificationRepo
{
    
}