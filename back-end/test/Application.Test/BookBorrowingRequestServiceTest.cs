using System.Linq.Expressions;
using Application.Services.BookBorrowingRequests;
using Contract.Dtos.BookBorrowRequestDetails.Requests;
using Contract.Dtos.BookBorrowRequests.Requests;
using Contract.Dtos.BookBorrowRequests.Responses;
using Contract.Dtos.Common;
using Contract.Repositories;
using Contract.Shared;
using Contract.Shared.Constants;
using Contract.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Test;

public class BookBorrowingRequestServiceTest
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private List<BookBorrowingRequest> _bookRequests;
    private List<User> _users;
    private List<Book> _books;
    private List<BookBorrowingRequestDetail> _requestDetails;
    private BookBorrowingRequestService _service;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _bookRequests = new List<BookBorrowingRequest>();
        _users = new List<User>();
        _books = new List<Book>();
        _requestDetails = new List<BookBorrowingRequestDetail>();

        _unitOfWorkMock.SetupGet(u => u.BookBorrowingRequestRepository).Returns(Mock.Of<IBookBorrowingRequestRepository>());
        _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(Mock.Of<IUserRepository>());
        _unitOfWorkMock.SetupGet(u => u.BookRepository).Returns(Mock.Of<IBookRepository>());
        _unitOfWorkMock.SetupGet(u => u.BookBorrowingRequestDetailRepository).Returns(Mock.Of<IBookBorrowingRequestDetailRepository>());

        _service = new BookBorrowingRequestService(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GetListBookBorrowingRequestAsync_ValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var request = new GetListBookBorrowingRequestFilterRequestDto
        {
            SearchRequestor = "user",
            SearchStatus = Status.Waiting,
            PageIndex = 1,
            PageSize = 2
        };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        var bookRequest = new BookBorrowingRequest
        {
            Id = 1,
            Requestor = user,
            Approver = null,
            Status = Status.Waiting,
            DateRequested = DateTime.Today,
            BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>()
        };
        var paginatedList = new PaginatedList<GetListBookBorrowingRequestFilterResponseDto>(
            new List<GetListBookBorrowingRequestFilterResponseDto>
            {
                new GetListBookBorrowingRequestFilterResponseDto
                {
                    Id = 1,
                    Requestor = "user1",
                    Approver = null,
                    Status = "Waiting",
                    DateRequested = DateOnly.FromDateTime(DateTime.Today)
                }
            }, 1, 1, 2);

        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetListAsync(
            It.IsAny<QueryOptions<BookBorrowingRequest>>(),
            It.IsAny<Expression<Func<BookBorrowingRequest, GetListBookBorrowingRequestFilterResponseDto>>>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetListBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
            Assert.That(result.Value.Items[0].Requestor, Is.EqualTo("user1"));
            Assert.That(result.Value.TotalCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddNewBookBorrowingRequestAsync_ValidRequest_CreatesRequest()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 }
            }
        };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        var book = new Book { Id = 1, Available = 1 };
        var bookRequest = new BookBorrowingRequest { Id = 0, RequestorId = 1 };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(1))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetAllAsync<Book>(
            It.IsAny<Expression<Func<Book, Book>>>(),
            It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book> { book });
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.AddAsync(It.IsAny<BookBorrowingRequest>()))
            .Callback<BookBorrowingRequest>(br =>
            {
                br.Id = 1; // Simulate database ID assignment
                _bookRequests.Add(br);
            });
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestDetailRepository.AddRangeAsync(It.IsAny<List<BookBorrowingRequestDetail>>()))
            .Callback<List<BookBorrowingRequestDetail>>(details => _requestDetails.AddRange(details));
        _unitOfWorkMock.Setup(u => u.BookRepository.Update(It.IsAny<Book>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AddNewBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(1u));
            Assert.That(_bookRequests, Has.Count.EqualTo(1));
            Assert.That(_bookRequests[0].Id, Is.EqualTo(1u));
            Assert.That(_bookRequests[0].RequestorId, Is.EqualTo(1));
            Assert.That(_requestDetails, Has.Count.EqualTo(1));
            Assert.That(_requestDetails[0].BookId, Is.EqualTo(1));
            Assert.That(book.Available, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task AddNewBookBorrowingRequestAsync_InvalidBookIds_ReturnsFailure()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 2 }
            }
        };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        var book = new Book { Id = 1, Available = 1 };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(1))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetAllAsync<Book>(
            It.IsAny<Expression<Func<Book, Book>>>(),
            It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book> { book }); // Only book with ID 1 is available

        // Act
        var result = await _service.AddNewBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("The following book IDs are invalid or unavailable: 2"));
            Assert.That(_bookRequests, Is.Empty);
            Assert.That(_requestDetails, Is.Empty);
            Assert.That(book.Available, Is.EqualTo(1)); // Book availability unchanged
        });
    }

    [Test]
    public void AddNewBookBorrowingRequestAsync_SaveChangesFails_ThrowsException()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 }
            }
        };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        var book = new Book { Id = 1, Available = 1 };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(1))
            .ReturnsAsync(0);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetAllAsync<Book>(
            It.IsAny<Expression<Func<Book, Book>>>(),
            It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book> { book });
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.AddAsync(It.IsAny<BookBorrowingRequest>()))
            .Callback<BookBorrowingRequest>(br =>
            {
                br.Id = 1;
                _bookRequests.Add(br);
            });
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestDetailRepository.AddRangeAsync(It.IsAny<List<BookBorrowingRequestDetail>>()))
            .Callback<List<BookBorrowingRequestDetail>>(details => _requestDetails.AddRange(details));
        _unitOfWorkMock.Setup(u => u.BookRepository.Update(It.IsAny<Book>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _service.AddNewBookBorrowingRequestAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Failed to create book borrowing request: Database error"));
    }

    [Test]
    public async Task AddNewBookBorrowingRequestAsync_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto { RequestorId = 1 };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _service.AddNewBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Requestor not found or not authorized"));
        });
    }

    [Test]
    public async Task AddNewBookBorrowingRequestAsync_ExceedsMonthlyLimit_ReturnsFailure()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto { RequestorId = 1 };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(1))
            .ReturnsAsync(3);

        // Act
        var result = await _service.AddNewBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Number request in month exceeds!"));
        });
    }

    [Test]
    public async Task AddNewBookBorrowingRequestAsync_InvalidBookCount_ReturnsFailure()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 2 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 3 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 4 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 5 },
                new AddNewBookBorrowingRequestDetailDto { BookId = 6 }
            }
        };
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(1, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(1))
            .ReturnsAsync(0);

        // Act
        var result = await _service.AddNewBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("You must request between 1 and 5 books"));
        });
    }

    [Test]
    public async Task GetListBookRequestByRequestor_ValidRequestor_ReturnsRequestList()
    {
        // Arrange
        var requestorId = 1u;
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        var requests = new List<GetBookBorrowingRequestResponseDto>
        {
            new GetBookBorrowingRequestResponseDto { Id = 1 }
        };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(requestorId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetListByRequestorIdAsync(requestorId))
            .ReturnsAsync(requests);

        // Act
        var result = await _service.GetListBookRequestByRequestor(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Has.Count.EqualTo(1));
            Assert.That(result.Value![0].Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetListBookRequestByRequestor_InvalidRequestor_ReturnsFailure()
    {
        // Arrange
        var requestorId = 1u;
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(requestorId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _service.GetListBookRequestByRequestor(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Requestor not found or not authorized"));
        });
    }

    [Test]
    public async Task GetUserMonthRequest_ValidRequestor_ReturnsRequestCount()
    {
        // Arrange
        var requestorId = 1u;
        var user = new User { Id = 1, Username = "user1", Role = new Role { Name = RoleConstant.NORMAL_USER } };
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(requestorId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetCurrentRequestThisMonthByRequestorAsync(requestorId))
            .ReturnsAsync(2);

        // Act
        var result = await _service.GetUserMonthRequest(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetUserMonthRequest_InvalidRequestor_ReturnsFailure()
    {
        // Arrange
        var requestorId = 1u;
        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(requestorId, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _service.GetUserMonthRequest(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Requestor not found or not authorized"));
        });
    }

    [Test]
    public async Task UpdateRequestStatusAsync_ValidApproval_UpdatesStatus()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookStatusRequestDto { Status = Status.Approved, ApproverId = 2 };
        var bookRequest = new BookBorrowingRequest
        {
            Id = 1,
            Status = Status.Waiting,
            BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>()
        };
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetByIdAsync(id, It.IsAny<Expression<Func<BookBorrowingRequest, object>>>()))
            .ReturnsAsync(bookRequest);
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.Update(It.IsAny<BookBorrowingRequest>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateRequestStatusAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That(bookRequest.Status, Is.EqualTo(Status.Approved));
            Assert.That(bookRequest.ApproverId, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task UpdateRequestStatusAsync_NonExistentRequest_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookStatusRequestDto { Status = Status.Approved, ApproverId = 2 };
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetByIdAsync(id, It.IsAny<Expression<Func<BookBorrowingRequest, object>>>()))
            .ReturnsAsync((BookBorrowingRequest)null!);

        // Act
        var result = await _service.UpdateRequestStatusAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Book borrowing request not found"));
        });
    }

    [Test]
    public async Task UpdateRequestStatusAsync_NonWaitingStatus_ReturnsFailure()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookStatusRequestDto { Status = Status.Approved, ApproverId = 2 };
        var bookRequest = new BookBorrowingRequest
        {
            Id = 1,
            Status = Status.Approved,
            BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>()
        };
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetByIdAsync(id, It.IsAny<Expression<Func<BookBorrowingRequest, object>>>()))
            .ReturnsAsync(bookRequest);

        // Act
        var result = await _service.UpdateRequestStatusAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Request is not in Waiting status"));
        });
    }

    [Test]
    public async Task UpdateRequestStatusAsync_RejectedStatus_RestoresBookAvailability()
    {
        // Arrange
        var id = 1u;
        var request = new UpdateBookStatusRequestDto { Status = Status.Rejected, ApproverId = 2 };
        var bookRequest = new BookBorrowingRequest
        {
            Id = 1,
            Status = Status.Waiting,
            BookBorrowingRequestDetails = new List<BookBorrowingRequestDetail>
            {
                new BookBorrowingRequestDetail { BookId = 1 }
            }
        };
        var book = new Book { Id = 1, Available = 0 };
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.GetByIdAsync(id, It.IsAny<Expression<Func<BookBorrowingRequest, object>>>()))
            .ReturnsAsync(bookRequest);
        _unitOfWorkMock.Setup(u => u.BookRepository.GetAllAsync<Book>(
            It.IsAny<Expression<Func<Book, Book>>>(),
            It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book> { book });
        _unitOfWorkMock.Setup(u => u.BookRepository.Update(It.IsAny<Book>()));
        _unitOfWorkMock.Setup(u => u.BookBorrowingRequestRepository.Update(It.IsAny<BookBorrowingRequest>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateRequestStatusAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That(bookRequest.Status, Is.EqualTo(Status.Rejected));
            Assert.That(book.Available, Is.EqualTo(1));
        });
    }
}
