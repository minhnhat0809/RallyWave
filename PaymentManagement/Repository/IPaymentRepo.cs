using Entity;

namespace PaymentManagement.Repository;

public interface IPaymentRepo : IRepositoryBase<PaymentDetail>
{
    Task DeletePayment(PaymentDetail paymentDetail);
}