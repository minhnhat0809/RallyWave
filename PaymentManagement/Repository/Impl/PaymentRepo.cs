using Entity;

namespace PaymentManagement.Repository.Impl;

public class PaymentRepo(RallyWaveContext repositoryContext) : RepositoryBase<PaymentDetail>(repositoryContext), IPaymentRepo
{
    
}