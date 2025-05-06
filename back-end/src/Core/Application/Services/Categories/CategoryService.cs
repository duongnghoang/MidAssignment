using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Dtos.Common;
using Contract.Shared;
using Contract.UnitOfWork;
using Domain.Entities;

namespace Application.Services.Categories;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<Result<List<GetCategoryResponseDto>>> GetAllCategories()
    {
        var result = await unitOfWork.CategoryRepository.GetAllAsync(category => new GetCategoryResponseDto(category.Id, category.Name));

        return Result.Success(result);
    }

    public async Task<Result<PaginatedList<GetCategoryResponseDto>>> GetListCategoriesFilterAsync(
        GetAllCategoryFilterRequestDto request)
    {
        var queryOptions = new QueryOptions<Category>();
        if (!string.IsNullOrEmpty(request.Name))
        {
            queryOptions.AddFilter(c => c.Name.Contains(request.Name));
        }
        queryOptions.PageSize = request.PageSize;
        queryOptions.PageIndex = request.PageIndex;

        var result =
            await unitOfWork.CategoryRepository.GetListAsync(queryOptions, category => new GetCategoryResponseDto(category.Id, category.Name));

        return Result.Success(result);
    }

    public async Task<Result<uint>> AddNewCategoryAsync(AddCategoryRequestDto request)
    {
        var category = request.ToCategory(); 
        await unitOfWork.CategoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(category.Id);
    }

    public async Task<Result> UpdateCategoryAsync(uint id, UpdateCategoryRequestDto request)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Result.Failure("Category not found");
        }
        category.Name = request.Name;
        unitOfWork.CategoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<bool>> DeleteCategoryAsync(uint id)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Result.Failure<bool>("Category not found!");
        }

        var hasBooks = await unitOfWork.BookRepository.IsInCategoryAsync(id);
        if (hasBooks)
        {
            return Result.Failure<bool>("Cannot delete category because it contains books!");
        }

        unitOfWork.CategoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(true);
    }
}