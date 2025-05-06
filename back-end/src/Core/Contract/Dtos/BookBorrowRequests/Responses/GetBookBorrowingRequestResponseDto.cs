using Contract.Dtos.Books.Responses;

namespace Contract.Dtos.BookBorrowRequests.Responses;

public class GetBookBorrowingRequestResponseDto
{
    public uint Id { get; set; }
    public string? Requestor { get; set; }
    public string? Approver { get; set; }
    public DateOnly DateRequested { get; set; }
    public string? Status { get; set; }
    public List<GetBookResponseDto> BookRequested { get; set; } = [];
}