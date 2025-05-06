using Domain.Abstractions.Base;

namespace Domain.Entities;

public class Book : BaseEntity
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public DateOnly PublicationDate { get; set; }
    public uint Quantity { get; set; }
    public uint Available { get; set; }
    public uint CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    public Category? Category { get; set; }
    public ICollection<BookBorrowingRequestDetail>? BookBorrowingRequestDetails { get; set; }
}