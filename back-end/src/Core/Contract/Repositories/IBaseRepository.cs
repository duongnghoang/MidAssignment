using Contract.Dtos.Common;
using Contract.Shared;
using Domain.Abstractions.Base;
using System.Linq.Expressions;

namespace Contract.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<List<TResponse>> GetAllAsync<TResponse>(Expression<Func<TEntity, TResponse>> select, Expression<Func<TEntity, bool>>? where = null);
    Task<PaginatedList<TResponse>> GetListAsync<TResponse>(QueryOptions<TEntity> queryOptions,
        Expression<Func<TEntity, TResponse>> select) where TResponse : class;
    Task<TEntity?> GetByIdAsync(uint id, Expression<Func<TEntity, object>>? includeExpressions = null);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);
}