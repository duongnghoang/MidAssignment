using Domain.Abstractions.Base;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public uint RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public ICollection<BookBorrowingRequest>? BookBorrowingRequests { get; set; }
    public ICollection<BookBorrowingRequest>? BookBorrowingApproves { get; set; }
}