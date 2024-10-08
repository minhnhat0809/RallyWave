using System.Linq.Expressions;

namespace UserManagement.Repository;

public interface IRepositoryBase<T>
{
    Task<List<TResult>> FindAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);

    Task<List<TResult>> FindByConditionAsync<TResult>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);

    Task<TResult?> GetByIdAsync<TResult>(
        object id,
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);
    Task<bool> CreateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> SaveChange();
} 