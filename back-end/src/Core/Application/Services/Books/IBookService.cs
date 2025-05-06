using Contract.Dtos.Books.Requests;
using Contract.Dtos.Books.Responses;
using Contract.Shared;

namespace Application.Services.Books;

public interface IBookService
{
    Task<Result<PaginatedList<GetAllBooksResponseDto>>> GetListBooksFilterAsync(GetAllBooksFilterRequestDto request);
    Task<Result<uint>> AddBookAsync(AddBookRequestDto request);
    Task<Result<bool>> UpdateBookAsync(uint id, UpdateBookRequestDto request);
    Task<Result<bool>> DeleteBookAsync(uint id);
}