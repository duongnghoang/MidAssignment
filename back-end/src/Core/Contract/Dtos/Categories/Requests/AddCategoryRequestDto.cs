using Domain.Entities;

namespace Contract.Dtos.Categories.Requests;

public record AddCategoryRequestDto(string Name)
{
    public Category ToCategory()
    {
        return new Category
        {
            Name = Name
        };
    }
}