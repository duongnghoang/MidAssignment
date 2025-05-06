using Application.Services.Books;
using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync()
    {
        var result = await categoryService.GetAllCategories();

        return Ok(result);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetListCategoriesFilterAsync([FromQuery] GetAllCategoryFilterRequestDto request)
    {
        var result = await categoryService.GetListCategoriesFilterAsync(request);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewCategoryAsync([FromBody] AddCategoryRequestDto request)
    {
        var result = await categoryService.AddNewCategoryAsync(request);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategoryAsync(uint id, [FromBody] UpdateCategoryRequestDto request)
    {
        var result = await categoryService.UpdateCategoryAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoryAsync(uint id)
    {
        var result = await categoryService.DeleteCategoryAsync(id);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}