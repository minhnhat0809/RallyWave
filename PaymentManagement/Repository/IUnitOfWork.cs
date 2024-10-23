namespace PaymentManagement.Repository;

public interface IUnitOfWork
{
    IPaymentRepo PaymentRepo { get; }
}