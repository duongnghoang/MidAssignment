using Domain.Enums;

namespace Contract.Dtos.BookBorrowRequests.Requests;

public class UpdateBookStatusRequestDto
{
    public Status Status { get; set; }
    public uint ApproverId { get; set; }
}