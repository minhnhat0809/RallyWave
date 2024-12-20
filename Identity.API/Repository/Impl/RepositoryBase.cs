﻿using System.Linq.Expressions;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Repository.Impl;

public class RepositoryBase<T>(RallyWaveContext repositoryContext) : IRepositoryBase<T>
    where T : class
{
    public async Task<List<TResult>> FindAllAsync<TResult>(
        Expression<Func<T, TResult>> selector, 
        params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

        if (includes is { Length: > 0 })
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();
        }

        return await query.Select(selector).ToListAsync(); 
    }


    
    public async Task<List<TResult>> FindByConditionAsync<TResult>(
        Expression<Func<T, bool>> expression, 
        Expression<Func<T, TResult>> selector, 
        params Expression<Func<T, object>>[]? includes)
    {
        var query = repositoryContext.Set<T>().Where(expression);

        if (includes is { Length: > 0 })
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();
        }

        return await query.Select(selector).ToListAsync(); 
    }


    
    public async Task<TResult?> GetByIdAsync<TResult>(
        object id, 
        Expression<Func<T, TResult>> selector, 
        params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

        if (includes is { Length: > 0 })
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();
        }

        return await query.Where(e => EF.Property<object>(e, "UserId") == id)
            .Select(selector)
            .FirstOrDefaultAsync();
    }


    public async Task<bool> CreateAsync(T entity)
    {
        try
        {
            await repositoryContext.Set<T>().AddAsync(entity);
            return await SaveChange();
        } catch (Exception ex)
        {
            throw new Exception("Fail to add",ex);
        }
    }
    public async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            repositoryContext.Set<T>().Update(entity);
            return await SaveChange();
        }
        catch (Exception ex)
        {
            throw new Exception("Fail to update", ex);
        }
    }
    public async Task<bool> DeleteAsync(T entity)
    {
        try
        {
            repositoryContext.Set<T>().Update(entity);
            return await SaveChange();
        }
        catch (Exception ex)
        {
            throw new Exception("Fail to remove", ex);
        }
    }
    public async Task<bool> SaveChange()
    {
        var result = await repositoryContext.SaveChangesAsync();
        return result > 0;
    }
}