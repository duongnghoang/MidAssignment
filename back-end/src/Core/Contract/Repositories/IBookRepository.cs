using Domain.Entities;

namespace Contract.Repositories;

public interface IBookRepository : IBaseRepository<Book>
{
    Task<Book?> GetByISBNAsync(string isbn);
    Task<bool> IsInCategoryAsync(uint id);
}