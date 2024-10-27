using Entity;

namespace PaymentManagement.Repository.Impl;

public class SubscriptionRepo(RallyWaveContext repositoryContext) : RepositoryBase<Subscription>(repositoryContext), ISubscriptionRepo
{
    
}