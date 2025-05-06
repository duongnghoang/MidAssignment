using Application.Services.Books;
using Contract.Dtos.Books.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(IBookService bookService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListBooksFilterAsync([FromQuery] GetAllBooksFilterRequestDto request)
    {
        var result = await bookService.GetListBooksFilterAsync(request);

        return Ok(result);
    }

    [Authorize("SuperUser")]
    [HttpPost]
    public async Task<IActionResult> AddBookAsync([FromBody] AddBookRequestDto request)
    {
        var result = await bookService.AddBookAsync(request);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize("SuperUser")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(uint id, [FromBody] UpdateBookRequestDto request)
    {
        var result = await bookService.UpdateBookAsync(id, request);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    [Authorize("SuperUser")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(uint id)
    {
        var result = await bookService.DeleteBookAsync(id);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}