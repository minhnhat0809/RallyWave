namespace UserManagement.Repository;

public interface IUnitOfWork
{ 
    IUserRepo UserRepo { get; }
}