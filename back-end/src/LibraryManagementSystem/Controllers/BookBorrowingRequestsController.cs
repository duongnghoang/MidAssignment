using Application.Services.BookBorrowingRequests;
using Contract.Dtos.BookBorrowRequests.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookBorrowingRequestsController(IBookBorrowingRequestService bookBorrowingRequestService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetListBookBorrowingRequestsFilterAsync([FromQuery] GetListBookBorrowingRequestFilterRequestDto request)
    {
        var result = await bookBorrowingRequestService.GetListBookBorrowingRequestAsync(request);

        return Ok(result);
    }

    [Authorize("NormalUser")]
    [HttpPost]
    public async Task<IActionResult> AddBookBorrowingRequestAsync([FromBody] AddNewBookBorrowingRequestDto request)
    {
        var result = await bookBorrowingRequestService.AddNewBookBorrowingRequestAsync(request);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("requestor/{requestId}")]
    public async Task<IActionResult> GetBookBorrowingRequestByRequestorIdAsync(uint requestId)
    {
        var result = await bookBorrowingRequestService.GetListBookRequestByRequestor(requestId);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("month-request")]
    public async Task<IActionResult> GetUserMonthRequest([FromQuery] uint requestorId)
    {
        var result = await bookBorrowingRequestService.GetUserMonthRequest(requestorId);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize("SuperUser")]
    [HttpPut("{id}/update-status")]
    public async Task<IActionResult> UpdateRequestStatus(uint id, [FromBody] UpdateBookStatusRequestDto request)
    {
        var result = await bookBorrowingRequestService.UpdateRequestStatusAsync(id, request);

        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}