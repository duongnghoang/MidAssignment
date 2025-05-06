using Contract.Shared.Constants;
using Domain.Enums;

namespace Contract.Dtos.BookBorrowRequests.Requests;

public class GetListBookBorrowingRequestFilterRequestDto
{
    public string? SearchRequestor { get; set; }
    public Status? SearchStatus { get; set; }
    public int PageSize { get; set; } = PaginationConstant.DefaultPageSize;
    public int PageIndex { get; set; } = PaginationConstant.DefaultPageIndex;
}