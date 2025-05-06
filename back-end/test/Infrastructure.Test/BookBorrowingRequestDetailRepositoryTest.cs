using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Test;

[TestFixture]
public class BookBorrowingRequestDetailRepositoryTest
{
    private ApplicationDbContext _context; 
    private BookBorrowingRequestDetailRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed data
        var data = new List<BookBorrowingRequestDetail>
        {
            new BookBorrowingRequestDetail { Id = 1, BookId = 1 },
            new BookBorrowingRequestDetail { Id = 2, BookId = 2 },
            new BookBorrowingRequestDetail { Id = 3, BookId = 3 }
        };
        _context.BookBorrowingRequestsDetail.AddRange(data);
        _context.SaveChanges();

        _repository = new BookBorrowingRequestDetailRepository(_context);
    }

    [Test]
    public async Task GetByBookIdAsync_ValidBookId_ReturnsBookBorrowingRequestDetail()
    {
        // Arrange
        var bookId = 1u;

        // Act
        var result = await _repository.GetByBookIdAsync(bookId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1u));
            Assert.That(result.BookId, Is.EqualTo(bookId));
        });
    }

    [Test]
    public async Task GetByBookIdAsync_NonExistentBookId_ReturnsNull()
    {
        // Arrange
        var bookId = 999u;

        // Act
        var result = await _repository.GetByBookIdAsync(bookId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}