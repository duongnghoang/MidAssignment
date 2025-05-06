using Contract.Dtos.Common;
using Contract.Repositories;
using Contract.Shared;
using Domain.Abstractions.Base;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class BaseRepository<TEntity>(ApplicationDbContext context) : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext _context = context;

    public virtual async Task<List<TResponse>> GetAllAsync<TResponse>(Expression<Func<TEntity, TResponse>> select, Expression<Func<TEntity, bool>>? where = null)
    {
        var response = _context.Set<TEntity>().AsQueryable();
        if (where != null)
        {
            response = response.Where(where);
        }

        return await response.Select(select).ToListAsync(); ;
    }

    public virtual async Task<PaginatedList<TResponse>> GetListAsync<TResponse>(QueryOptions<TEntity> queryOptions,
        Expression<Func<TEntity, TResponse>> select) where TResponse : class
    {
        var template = _context.Set<TEntity>().AsQueryable();
        if (queryOptions.Filters.Count > 0)
        {
            template = queryOptions.Filters.Aggregate(template, (current, filter) => current.Where(filter));
        }
        if (queryOptions.IncludeExpressions.Count > 0)
        {
            template = queryOptions.IncludeExpressions.Aggregate(template, (current, includeExpression) => current.Include(includeExpression));
        }

        var responses = template.OrderBy(queryOptions.OrderBy).Select(select);
        var count = await responses.CountAsync();
        var paginatedResponse = await responses.Skip((queryOptions.PageIndex - 1) * queryOptions.PageSize)
            .Take(queryOptions.PageSize).ToListAsync();

        return new PaginatedList<TResponse>(paginatedResponse, count, queryOptions.PageIndex, queryOptions.PageSize);
    }

    public virtual async Task<TEntity?> GetByIdAsync(uint id, Expression<Func<TEntity, object>>? includeExpressions = null)
    {
        var template = _context.Set<TEntity>().AsQueryable();
        if (includeExpressions is not null)
        {
           template = template.Include(includeExpressions);
        }
        var entity = await template.FirstOrDefaultAsync(e => e.Id == id);

        return entity;
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(List<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

}