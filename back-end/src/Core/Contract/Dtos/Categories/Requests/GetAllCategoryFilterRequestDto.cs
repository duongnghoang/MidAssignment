using Contract.Shared.Constants;

namespace Contract.Dtos.Categories.Requests;

public class GetAllCategoryFilterRequestDto
{
    public string? Name { get; set; }
    public int PageSize { get; set; } = PaginationConstant.DefaultPageSize;
    public int PageIndex { get; set; } = PaginationConstant.DefaultPageIndex;
}