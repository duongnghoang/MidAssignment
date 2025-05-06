using Domain.Abstractions.Base;
using Domain.Enums;

namespace Domain.Entities;

public class BookBorrowingRequest : BaseEntity
{
    public uint RequestorId { get; set; }
    public uint? ApproverId { get; set; }
    public Status Status { get; set; }
    public DateTime DateRequested { get; set; }
    public User? Requestor { get; set; }
    public User? Approver { get; set; }
    public ICollection<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = [];
}