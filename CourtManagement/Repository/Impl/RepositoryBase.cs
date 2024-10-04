using System.Linq.Expressions;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace CourtManagement.Repository.Impl;

public class RepositoryBase<T>(RallywaveContext repositoryContext) : IRepositoryBase<T>
    where T : class
{
    public Task<List<T>> FindAllAsync (params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

        query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();

        return query.ToListAsync();
    }

    
    public Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        var query = repositoryContext.Set<T>().Where(expression);

        query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();

        return query.ToListAsync();
    }

    
    public async Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = repositoryContext.Set<T>();

        query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();

        return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id") == id);
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