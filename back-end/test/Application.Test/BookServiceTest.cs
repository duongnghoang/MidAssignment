using System.Linq.Expressions;
using Application.Services.Books;
using Contract.Dtos.Books.Requests;
using Contract.Dtos.Books.Responses;
using Contract.Dtos.Common;
using Contract.Repositories;
using Contract.Shared;
using Contract.UnitOfWork;
using Domain.Entities;
using Moq;

namespace Application.Test;

[TestFixture]
public class BookServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private BookService _service;
    private List<Book> _books;
    private List<Category> _categories;
    private List<BookBorrowingRequestDetail> _requestDetails;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _books = new List<Book>();
        _categories = new List<Category>();
        _requestDetails = new List<BookBorrowingRequestDetail>();

        _unitOfWorkMock.SetupGet(u => u.BookRepository).Returns(Mock.Of<IBookRepository>());
        _unitOfWorkMock.SetupGet(u => u.CategoryRepository).Returns(Mock.Of<ICategoryRepository>());
        _unitOfWorkMock.SetupGet(u => u.BookBorrowingRequestDetailRepository).Returns(Mock.Of<IBookBorrowingRequestDetailRepository>());

        _service = new BookService(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GetListBooksFilterAsync_ValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var request = new GetAllBooksFilterRequestDto
        {
            CategoryId = 1,
            SearchString = "test",
            IsAvailable = true,
            PageIndex = 1,
            PageSize = 2
        };
        var category = new Category { Id = 1, Name = "Fiction" };
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 5,
            Available = 3,
            CategoryId = 1,
            Category = category
        };
        var paginatedList = new PaginatedList<GetAllBooksResponseDto>(
            new List<GetAllBooksResponseDto>
            {
                new GetAllBooksResponseDto
                {
                    Id = 1,
                    Title = "Test Book",
                    Author = "Test Author",
                    ISBN = "1234567890",
                    PublicationDate = DateOnly.FromDateTime(DateTime.Today),
                    Quantity = 5,
                    Available = 3,
                    CategoryId = 1,
                    Category = "Fiction"
                }
            }, 1, 1, 2);

        _unitOfWorkMock.Setup(u => u.BookRepository.GetListAsync(
                It.IsAny<QueryOptions<Book>>(),
                It.IsAny<Expression<Func<Book, GetAllBooksResponseDto>>>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetListBooksFilterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items[0].Title, Is.EqualTo("Test Book"));
            Assert.That(result.Value.TotalCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddBookAsync_ValidRequest_AddsBook()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            Title = "New Book",
            Author = "New Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 10,
            CategoryId = 1
        };
        var category = new Category { Id = 1, Name = "Fiction" };
        var book = request.ToBook();

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync((Book)null!);
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(1, null))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.BookRepository.AddAsync(It.IsAny<Book>()))
            .Callback<Book>(b => _books.Add(b));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AddBookAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(1u));
            Assert.That(_books, Has.Count.EqualTo(1));
            Assert.That(_books[0].ISBN, Is.EqualTo("0987654321"));
        });
    }

    [Test]
    public async Task AddBookAsync_DuplicateISBN_ReturnsFailure()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            ISBN = "1234567890",
            CategoryId = 1
        };
        var existingBook = new Book { Id = 1, ISBN = "1234567890" };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync(existingBook);

        // Act
        var result = await _service.AddBookAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("A book with this ISBN already exists!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task AddBookAsync_InvalidCategory_ReturnsFailure()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            ISBN = "0987654321",
            CategoryId = 1
        };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync((Book)null!);
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(1, null))
            .ReturnsAsync((Category)null!);

        // Act
        var result = await _service.AddBookAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Book category does not exist!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task UpdateBookAsync_ValidRequest_UpdatesBook()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 15,
            Available = 12,
            CategoryId = 1
        };
        var book = new Book { Id = 1, ISBN = "1234567890", CategoryId = 2 };
        var category = new Category { Id = 1, Name = "Fiction" };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id,null))
            .ReturnsAsync(book);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync((Book)null!);
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(1, null))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.BookRepository.Update(It.IsAny<Book>()))
            .Callback<Book>(b => _books.Add(b));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateBookAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That(_books, Has.Count.EqualTo(1));
            Assert.That(_books[0].ISBN, Is.EqualTo("0987654321"));
            Assert.That(_books[0].CategoryId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateBookAsync_NonExistentBook_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookRequestDto { ISBN = "0987654321", CategoryId = 1 };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync((Book)null!);

        // Act
        var result = await _service.UpdateBookAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Book not found!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task UpdateBookAsync_DuplicateISBN_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookRequestDto { ISBN = "1234567890", CategoryId = 1 };
        var book = new Book { Id = 1, ISBN = "0987654321" };
        var existingBook = new Book { Id = 2, ISBN = "1234567890" };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync(book);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync(existingBook);

        // Act
        var result = await _service.UpdateBookAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("A book with this ISBN already exists!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task UpdateBookAsync_InvalidCategory_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookRequestDto { ISBN = "0987654321", CategoryId = 1 };
        var book = new Book { Id = 1, ISBN = "1234567890" };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync(book);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetByISBNAsync(request.ISBN))
            .ReturnsAsync((Book)null!);
        _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(1, null))
            .ReturnsAsync((Category)null!);

        // Act
        var result = await _service.UpdateBookAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Book category does not exist!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task DeleteBookAsync_ValidRequest_DeletesBook()
    {
        // Arrange
        const uint id = 1u;
        var book = new Book { Id = 1, ISBN = "1234567890" };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync(book);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestDetailRepository.GetByBookIdAsync(id))
            .ReturnsAsync((BookBorrowingRequestDetail)null!);
        _unitOfWorkMock.Setup(u => u.BookRepository.Delete(It.IsAny<Book>()))
            .Callback<Book>(b => _books.Remove(b));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteBookAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task DeleteBookAsync_NonExistentBook_ReturnsFailure()
    {
        // Arrange
        var id = 1u;

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync((Book)null!);

        // Act
        var result = await _service.DeleteBookAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Book not found!"));
            Assert.That(_books, Is.Empty);
        });
    }

    [Test]
    public async Task DeleteBookAsync_BookWithBorrowedCopies_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var book = new Book { Id = 1, ISBN = "1234567890" };
        var requestDetail = new BookBorrowingRequestDetail { BookId = 1 };

        _unitOfWorkMock.Setup(u => u.BookRepository.GetByIdAsync(id, null))
            .ReturnsAsync(book);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestDetailRepository.GetByBookIdAsync(id))
            .ReturnsAsync(requestDetail);

        // Act
        var result = await _service.DeleteBookAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Cannot delete book while copies are borrowed!"));
            Assert.That(_books, Is.Empty);
        });
    }
}