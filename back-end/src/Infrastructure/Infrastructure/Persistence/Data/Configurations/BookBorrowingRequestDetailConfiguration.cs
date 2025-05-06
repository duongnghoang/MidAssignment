using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class BookBorrowingRequestDetailConfiguration : IEntityTypeConfiguration<BookBorrowingRequestDetail>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequestDetail> builder)
    {
        builder.HasKey(bookBorrowingRequestDetail => bookBorrowingRequestDetail.Id);

        builder.HasIndex(bookBorrowingRequestDetail =>
            new { bookBorrowingRequestDetail.BookId, bookBorrowingRequestDetail.RequestId }).IsUnique();

        builder.HasOne(bookBorrowingRequestDetail => bookBorrowingRequestDetail.Book)
            .WithMany(book => book.BookBorrowingRequestDetails)
            .HasForeignKey(bookBorrowingRequestDetail => bookBorrowingRequestDetail.BookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(bookBorrowingRequestDetail => bookBorrowingRequestDetail.BookBorrowingRequest)
            .WithMany(bookBorrowingRequest => bookBorrowingRequest.BookBorrowingRequestDetails)
            .HasForeignKey(bookBorrowingRequestDetail => bookBorrowingRequestDetail.RequestId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}