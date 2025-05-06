using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Shared;

namespace Application.Services.Categories;

public interface ICategoryService
{
    Task<Result<List<GetCategoryResponseDto>>> GetAllCategories();
    Task<Result<uint>> AddNewCategoryAsync(AddCategoryRequestDto request);
    Task<Result> UpdateCategoryAsync(uint id, UpdateCategoryRequestDto request);
    Task<Result<PaginatedList<GetCategoryResponseDto>>> GetListCategoriesFilterAsync(
        GetAllCategoryFilterRequestDto request);

    Task<Result<bool>> DeleteCategoryAsync(uint id);
}