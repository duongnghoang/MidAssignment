using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookBorrowingRequestDetailRepository(ApplicationDbContext context) : BaseRepository<BookBorrowingRequestDetail>(context), IBookBorrowingRequestDetailRepository
{
    public async Task<BookBorrowingRequestDetail?> GetByBookIdAsync(uint bookId)
    {
        return await context.BookBorrowingRequestsDetail.FirstOrDefaultAsync(rd => rd.BookId == bookId);

    }
}