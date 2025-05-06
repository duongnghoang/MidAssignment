using Contract.Dtos.BookBorrowRequests.Responses;
using Contract.Dtos.Books.Responses;
using Contract.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookBorrowingRequestRepository(ApplicationDbContext context) : BaseRepository<BookBorrowingRequest>(context), IBookBorrowingRequestRepository
{
    public async Task<List<GetBookBorrowingRequestResponseDto>> GetListByRequestorIdAsync(uint requestorId)
    {
        var response = await _context.Set<BookBorrowingRequest>().AsQueryable()
            .Include(br => br.Requestor)
            .Include(br => br.Approver)
            .Include(br => br.BookBorrowingRequestDetails)!
                .ThenInclude(rd => rd.Book)
            .Where(br => br.RequestorId == requestorId)
            .Where(br => br.DateRequested.Month == DateTime.Now.Month)
            .Select(r => new GetBookBorrowingRequestResponseDto
            {
                Id = r.Id,
                Requestor = r.Requestor!.Username,
                Approver = r.Approver != null ? r.Approver.Username : null,
                DateRequested = DateOnly.FromDateTime(r.DateRequested),
                Status = r.Status.ToString(),
                BookRequested = r.BookBorrowingRequestDetails
                    .Select(d => new GetBookResponseDto
                    {
                        Id = d.Book!.Id,
                        Title = d.Book.Title,
                        Author = d.Book.Author,
                        ISBN = d.Book.ISBN,
                        PublicationDate = d.Book.PublicationDate,
                        Quantity = d.Book.Quantity,
                        Available = d.Book.Available,
                        CategoryId = d.Book.CategoryId,
                        Category = d.Book.Category!.Name
                    })
                    .ToList()
            })
            .ToListAsync();

        return response;
    }

    public async Task<int> GetCurrentRequestThisMonthByRequestorAsync(uint requestorId)
    {
        var response = await _context.Set<BookBorrowingRequest>()
            .Where(br => br.RequestorId == requestorId)
            .Where(br => br.Status != Status.Rejected)
            .Where(br => br.DateRequested.Month == DateTime.Now.Month)
            .CountAsync();

        return response;
    }
}