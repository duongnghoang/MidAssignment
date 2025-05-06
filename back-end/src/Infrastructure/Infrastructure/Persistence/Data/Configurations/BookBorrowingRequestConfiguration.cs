using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class BookBorrowingRequestConfiguration : IEntityTypeConfiguration<BookBorrowingRequest>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequest> builder)
    {
        builder.HasKey(bookBorrowingRequest => bookBorrowingRequest.Id);

        builder.HasOne(bookBorrowingRequest => bookBorrowingRequest.Requestor)
            .WithMany(requestor => requestor.BookBorrowingRequests)
            .HasForeignKey(bookBorrowingRequest => bookBorrowingRequest.RequestorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(bookBorrowingRequest => bookBorrowingRequest.Approver)
            .WithMany(requestor => requestor.BookBorrowingApproves)
            .HasForeignKey(bookBorrowingRequest => bookBorrowingRequest.ApproverId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}