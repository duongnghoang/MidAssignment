using Domain.Abstractions.Base;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public uint RoleId { get; set; }
    public Role Role { get; set; }
    public ICollection<BookBorrowingRequest> BookBorrowingRequests { get; set; }
    public ICollection<BookBorrowingRequest> BookBorrowingApproves { get; set; }
}