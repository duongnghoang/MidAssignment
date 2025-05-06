using Contract.Dtos.BookBorrowRequests.Responses;
using Domain.Entities;

namespace Contract.Repositories;

public interface IBookBorrowingRequestRepository : IBaseRepository<BookBorrowingRequest>
{
    Task<List<GetBookBorrowingRequestResponseDto>> GetListByRequestorIdAsync(uint requestorId);
    Task<int> GetCurrentRequestThisMonthByRequestorAsync(uint requestorId);
}