using Entity;

namespace PaymentManagement.Repository.Impl;

public class PaymentRepo(RallyWaveContext repositoryContext) : RepositoryBase<PaymentDetail>(repositoryContext), IPaymentRepo
{
    public async Task DeletePayment(PaymentDetail paymentDetail)
    {
        repositoryContext.PaymentDetails.Remove(paymentDetail);

        await SaveChange();
    }
}