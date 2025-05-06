using Contract.Repositories;

namespace Contract.UnitOfWork;

public interface IUnitOfWork
{
    IBookRepository BookRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IUserRepository UserRepository { get; }
    IBookBorrowingRequestRepository BookBorrowingRequestRepository { get; }
    IBookBorrowingRequestDetailRepository BookBorrowingRequestDetailRepository { get; }
    Task<int> SaveChangesAsync();
}