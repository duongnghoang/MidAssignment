using Domain.Entities;

namespace Contract.Repositories;

public interface IBookBorrowingRequestDetailRepository : IBaseRepository<BookBorrowingRequestDetail>
{
    Task<BookBorrowingRequestDetail?> GetByBookIdAsync(uint bookId);
}