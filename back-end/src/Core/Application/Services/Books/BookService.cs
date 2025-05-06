using Contract.Dtos.Books.Requests;
using Contract.Dtos.Books.Responses;
using Contract.Dtos.Common;
using Contract.Shared;
using Contract.UnitOfWork;
using Domain.Entities;

namespace Application.Services.Books;

public class BookService(IUnitOfWork unitOfWork) : IBookService
{
    public async Task<Result<PaginatedList<GetAllBooksResponseDto>>> GetListBooksFilterAsync(GetAllBooksFilterRequestDto request)
    {
        var options = new QueryOptions<Book>();
        if (request.CategoryId != null)
        {
            options.AddFilter(b => b.CategoryId == request.CategoryId);
        }
        if (!string.IsNullOrEmpty(request.SearchString))
        {
            options.AddFilter(b => b.Title!.Contains(request.SearchString) ||
                                   b.ISBN!.Contains(request.SearchString) ||
                                   b.Author!.Contains(request.SearchString));
        }

        if (request.IsAvailable != null)
        {
            options.AddFilter(b => b.Available > 0);
        }
        options.OrderBy = b => b.Id;
        options.AddInclude(b => b.Category!);
        options.PageSize = request.PageSize;
        options.PageIndex = request.PageIndex;

        var result = await unitOfWork.BookRepository.GetListAsync(
            options, book => new GetAllBooksResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublicationDate = book.PublicationDate,
                Quantity = book.Quantity,
                Available = book.Available,
                CategoryId = book.CategoryId,
                Category = book.Category!.Name,
            });

        return Result.Success(result);
    }

    public async Task<Result<uint>> AddBookAsync(AddBookRequestDto request)
    {
        // Check if ISBN is unique
        var existingBook = await unitOfWork.BookRepository.GetByISBNAsync(request.ISBN);
        if (existingBook != null)
        {
            return Result.Failure<uint>("A book with this ISBN already exists!");
        }

        var category = await unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return Result.Failure<uint>("Book category does not exist!");
        }

        var book = request.ToBook();
        await unitOfWork.BookRepository.AddAsync(book);
        var result = await unitOfWork.SaveChangesAsync();

        return Result.Success((uint)result);
    }

    public async Task<Result<bool>> UpdateBookAsync(uint id, UpdateBookRequestDto request)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return Result.Failure<bool>("Book not found!");
        }

        // Check if ISBN is unique (excluding current book)
        var existingBook = await unitOfWork.BookRepository.GetByISBNAsync(request.ISBN);
        if (existingBook != null && existingBook.Id != id)
        {
            return Result.Failure<bool>("A book with this ISBN already exists!");
        }

        var category = await unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            return Result.Failure<bool>("Book category does not exist!");
        }

        request.ToBook(book);
        unitOfWork.BookRepository.Update(book);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(true);
    }

    public async Task<Result<bool>> DeleteBookAsync(uint id)
    {
        var book = await unitOfWork.BookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return Result.Failure<bool>("Book not found!");
        }

        var bookExist = await unitOfWork.BookBorrowingRequestDetailRepository.GetByBookIdAsync(book.Id);

        if (bookExist is not null)
        {
            return Result.Failure<bool>("Cannot delete book while copies are borrowed!");
        }

        unitOfWork.BookRepository.Delete(book);
        await unitOfWork.SaveChangesAsync();

        return Result.Success(true);
    }
}