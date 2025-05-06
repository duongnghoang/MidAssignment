using System.Linq.Expressions;
using Contract.Shared.Constants;
using Domain.Abstractions.Base;

namespace Contract.Dtos.Common;

public class QueryOptions<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// List of filters to apply to the search.
    /// </summary>
    public List<Expression<Func<TEntity, bool>>> Filters { get; } = [];
    /// <summary>
    /// List of include expression to apply to the search.
    /// </summary>
    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

    /// <summary>
    /// Order by expression to apply to the search.
    /// </summary>
    public Expression<Func<TEntity, object>> OrderBy { get; set; } = (p => p.Id);

    public int PageSize { get; set; } = PaginationConstant.DefaultPageSize;
    public int PageIndex { get; set; } = PaginationConstant.DefaultPageIndex;
    public void AddFilter(Expression<Func<TEntity, bool>> filter)
    {
        Filters.Add(filter);
    }
    public void AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        IncludeExpressions.Add(includeExpression);
    }
}