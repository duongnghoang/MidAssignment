using Contract.Dtos.BookBorrowRequestDetails.Requests;
using Domain.Entities;
using Domain.Enums;

namespace Contract.Dtos.BookBorrowRequests.Requests;

public class AddNewBookBorrowingRequestDto
{
    public uint RequestorId { get; set; }
    public List<AddNewBookBorrowingRequestDetailDto> RequestDetails { get; set; } = [];

    public BookBorrowingRequest ToBookBorrowingRequest()
    {
        return new BookBorrowingRequest
        {
            RequestorId = RequestorId,
            Status = Status.Waiting,
            DateRequested = DateTime.Today,
        };
    }
}