using Contract.Repositories;
using Contract.UnitOfWork;
using Infrastructure.Persistence.Data;

namespace Infrastructure.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IBookRepository BookRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IUserRepository UserRepository { get; }
    public IBookBorrowingRequestRepository BookBorrowingRequestRepository { get; }
    public IBookBorrowingRequestDetailRepository BookBorrowingRequestDetailRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context, 
        IBookRepository bookRepository, 
        ICategoryRepository categoryRepository, 
        IUserRepository userRepository, 
        IBookBorrowingRequestRepository bookBorrowingRequestRepository, 
        IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository)
    {
        _context = context;
        BookRepository = bookRepository;
        CategoryRepository = categoryRepository;
        UserRepository = userRepository;
        BookBorrowingRequestRepository = bookBorrowingRequestRepository;
        BookBorrowingRequestDetailRepository = bookBorrowingRequestDetailRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}