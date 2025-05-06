using Contract.Dtos.BookBorrowRequests.Requests;
using Contract.Dtos.BookBorrowRequests.Responses;
using Contract.Dtos.Common;
using Contract.Shared;
using Contract.Shared.Constants;
using Contract.UnitOfWork;
using Domain.Entities;
using System.Security.Claims;
using Domain.Enums;

namespace Application.Services.BookBorrowingRequests;

public class BookBorrowingRequestService(IUnitOfWork unitOfWork) : IBookBorrowingRequestService
{
    public async Task<Result<PaginatedList<GetListBookBorrowingRequestFilterResponseDto>>>
        GetListBookBorrowingRequestAsync(GetListBookBorrowingRequestFilterRequestDto request)
    {
        var queryOptions = new QueryOptions<BookBorrowingRequest>();

        queryOptions.AddInclude(bbr => bbr.Requestor);
        queryOptions.AddInclude(bbr => bbr.Approver);
        queryOptions.AddInclude(bbr => bbr.BookBorrowingRequestDetails);

        if (!string.IsNullOrEmpty(request.SearchRequestor))
        {
            queryOptions.AddFilter(bbr => bbr.Requestor.Username.Contains(request.SearchRequestor));
        }

        if (request.SearchStatus is not null)
        {
            queryOptions.AddFilter(bbr => bbr.Status.Equals(request.SearchStatus));
        }
        queryOptions.PageSize = request.PageSize;
        queryOptions.PageIndex = request.PageIndex;

        var result = await unitOfWork.BookBorrowingRequestRepository.GetListAsync(
            queryOptions, 
            bookBorrowingRequest => new GetListBookBorrowingRequestFilterResponseDto
            {
                Id = bookBorrowingRequest.Id,
                Requestor = bookBorrowingRequest.Requestor!.Username,
                Approver = bookBorrowingRequest.Approver != null ? bookBorrowingRequest.Approver.Username : null,
                Status = bookBorrowingRequest.Status.ToString(),
                DateRequested = DateOnly.FromDateTime(bookBorrowingRequest.DateRequested)
            });

        return Result.Success(result);
    }

    public async Task<Result<uint>> AddNewBookBorrowingRequestAsync(AddNewBookBorrowingRequestDto request)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(request.RequestorId, u => u.Role);
        if (user == null || user.Role.Name != RoleConstant.NORMAL_USER)
        {
            return Result.Failure<uint>("Requestor not found or not authorized");
        }

        var countInMonth =
            await unitOfWork.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(
                request.RequestorId);

        if (countInMonth >= 3)
        {
            return Result.Failure<uint>("Number request in month exceeds!");
        }

        var requestDetailCount = request.RequestDetails.Count;
        if (requestDetailCount is < 1 or > 5)
        {
            return Result.Failure<uint>("You must request between 1 and 5 books");
        }

        var requestedBookIds = request.RequestDetails.Select(rd => rd.BookId).ToList();

        var validBooks = await unitOfWork.BookRepository
            .GetAllAsync<Book>(b => b, b => requestedBookIds.Contains(b.Id) && b.Available > 0);

        var validBookIds = validBooks.Select(book => book.Id).ToList();

        var invalidBookIds = requestedBookIds.Except(validBookIds).ToList();
        if (invalidBookIds.Count > 0)
        {
            return Result.Failure<uint>(
                $"The following book IDs are invalid or unavailable: {string.Join(", ", invalidBookIds)}"
            );
        }

        try
        {
            // Create and save the book borrowing request
            var bookRequest = request.ToBookBorrowingRequest();
            await unitOfWork.BookBorrowingRequestRepository.AddAsync(bookRequest);
            await unitOfWork.SaveChangesAsync();

            // Create and save request details
            var bookRequestDetails = request.RequestDetails
                .Select(rd => new BookBorrowingRequestDetail
                {
                    RequestId = bookRequest.Id,
                    BookId = rd.BookId
                })
                .ToList();

            foreach (var book in validBooks)
            {
                book.Available -= 1;
                unitOfWork.BookRepository.Update(book);
            }
            await unitOfWork.BookBorrowingRequestDetailRepository.AddRangeAsync(bookRequestDetails);
            await unitOfWork.SaveChangesAsync();

            // Return the ID of the created request
            return Result.Success(bookRequest.Id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create book borrowing request: {ex.Message}");
        }
    }

    public async Task<Result<List<GetBookBorrowingRequestResponseDto>>> GetListBookRequestByRequestor(uint requestorId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(requestorId, u => u.Role);
        if (user == null || user.Role.Name != RoleConstant.NORMAL_USER)
        {
            return Result.Failure<List<GetBookBorrowingRequestResponseDto>>("Requestor not found or not authorized");
        }
        var result = await unitOfWork.BookBorrowingRequestRepository.GetListByRequestorIdAsync(requestorId);

        return Result.Success(result);
    }

    public async Task<Result<int>> GetUserMonthRequest(uint requestorId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(requestorId, u => u.Role);
        if (user == null || user.Role.Name != RoleConstant.NORMAL_USER)
        {
            return Result.Failure<int>("Requestor not found or not authorized");
        }
        var result =
            await unitOfWork.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(requestorId);

        return Result.Success(result);
    }

    public async Task<Result<bool>> UpdateRequestStatusAsync(uint id, UpdateBookStatusRequestDto request)
    {
        var currentRequest = await unitOfWork.BookBorrowingRequestRepository.GetByIdAsync(id,
            r => r.BookBorrowingRequestDetails);
        if (currentRequest == null)
        {
            return Result.Failure<bool>("Book borrowing request not found");
        }

        if (currentRequest.Status != Status.Waiting)
        {
            return Result.Failure<bool>("Request is not in Waiting status");
        }

        if (request.Status != Status.Approved && request.Status != Status.Rejected)
        {
            return Result.Failure<bool>("Invalid status provided");
        }
        try
        {
            if (request.Status == Status.Rejected)
            {
                var bookIds = currentRequest.BookBorrowingRequestDetails.Select(d => d.BookId).ToList();
                var books = await unitOfWork.BookRepository.GetAllAsync<Book>(
                    b => b,
                    b => bookIds.Contains(b.Id));

                foreach (var book in books)
                {
                    book.Available += 1;
                    unitOfWork.BookRepository.Update(book);
                }
            }

            // Update request status and approver
            currentRequest.Status = request.Status;
            currentRequest.ApproverId = request.ApproverId;
            unitOfWork.BookBorrowingRequestRepository.Update(currentRequest);
            await unitOfWork.SaveChangesAsync();

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to update book borrowing request status: {ex.Message}");
        }
    }
}