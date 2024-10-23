using Entity;

namespace PaymentManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IPaymentRepo PaymentRepo { get; } = new PaymentRepo(context);
}