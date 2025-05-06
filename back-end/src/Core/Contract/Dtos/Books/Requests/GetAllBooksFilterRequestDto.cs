using Contract.Shared.Constants;

namespace Contract.Dtos.Books.Requests;

public class GetAllBooksFilterRequestDto
{
    public int PageSize { get; set; } = PaginationConstant.DefaultPageSize;
    public int PageIndex { get; set; } = PaginationConstant.DefaultPageIndex;
    public uint? CategoryId { get; set; }
    public string? SearchString { get; set; }
    public bool? IsAvailable { get; set; }
}