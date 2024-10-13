using System.Linq.Expressions;

namespace BookingManagement.Repository;

public interface IRepositoryBase<T>
{
    Task<List<TResult>> FindAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);

    Task<List<TResult>> FindByConditionAsync<TResult>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);

    Task<TResult?> GetByConditionAsync<TResult>(
        Expression<Func<T, bool>> condition,
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);
    
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    Task<bool> CreateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> SaveChange();
} 