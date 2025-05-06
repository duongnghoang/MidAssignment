using Domain.Abstractions.Base;

namespace Domain.Entities;

public class BookBorrowingRequestDetail : BaseEntity
{
    public uint BookId { get; set; }
    public uint RequestId { get; set; }
    public BookBorrowingRequest? BookBorrowingRequest { get; set; }
    public Book? Book { get; set; }
}