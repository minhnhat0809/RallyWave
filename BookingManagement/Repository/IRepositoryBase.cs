using System.Linq.Expressions;

namespace BookingManagement.Repository;

public interface IRepositoryBase<T>
{
    Task<List<T>> FindAllAsync(params Expression<Func<T, object>>[] includes);
    Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes);
    Task<bool> CreateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> SaveChange();
}