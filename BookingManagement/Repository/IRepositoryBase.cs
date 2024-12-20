﻿using System.Linq.Expressions;

namespace BookingManagement.Repository;

public interface IRepositoryBase<T>
{
    Task<List<TResult>> FindAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        int pageNumber, 
        int pageSize,
        params Expression<Func<T, object>>[]? includes);

    Task<int> CountByConditionAsync(Expression<Func<T, bool>>? condition);

    Task<List<TResult>> FindByConditionAsync<TResult>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TResult>> selector,
        params Expression<Func<T, object>>[]? includes);

    Task<List<TResult>> FindByConditionWithSortingAndPagingAsync<TResult>(
        Expression<Func<T, bool>> expression, 
        Expression<Func<T, TResult>> selector, 
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, object>> orderBy, 
        Expression<Func<T, object>>? thenBy = null, 
        bool isAscending = true, 
        bool thenByAscending = true, 
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