using Contract.Dtos.BookBorrowRequests.Requests;
using Contract.Dtos.BookBorrowRequests.Responses;
using Contract.Shared;

namespace Application.Services.BookBorrowingRequests;

public interface IBookBorrowingRequestService
{
    Task<Result<PaginatedList<GetListBookBorrowingRequestFilterResponseDto>>>
        GetListBookBorrowingRequestAsync(GetListBookBorrowingRequestFilterRequestDto request);

    Task<Result<uint>> AddNewBookBorrowingRequestAsync(AddNewBookBorrowingRequestDto request);
    Task<Result<List<GetBookBorrowingRequestResponseDto>>> GetListBookRequestByRequestor(uint requestorId);
    Task<Result<int>> GetUserMonthRequest(uint requestorId);
    Task<Result<bool>> UpdateRequestStatusAsync(uint id, UpdateBookStatusRequestDto request);
}