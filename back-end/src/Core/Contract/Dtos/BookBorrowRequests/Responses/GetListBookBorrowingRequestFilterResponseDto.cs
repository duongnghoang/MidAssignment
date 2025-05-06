namespace Contract.Dtos.BookBorrowRequests.Responses;

public class GetListBookBorrowingRequestFilterResponseDto
{
    public string? Requestor { get; set; }
    public string? Approver { get; set; }
    public string? Status { get; set; }
    public DateOnly DateRequested { get; set; }
    public uint Id { get; set; }
}