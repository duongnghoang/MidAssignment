using Contract.Dtos.Common;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Test;

public class BaseRepositoryTest
{
    [TestFixture]
    public class BaseRepositoryTests
    {
        private Mock<ApplicationDbContext> _contextMock; 
        private BookBorrowingRequestDetailRepository _repository; 
        private List<BookBorrowingRequestDetail> _data;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _data = new List<BookBorrowingRequestDetail>
        {
            new BookBorrowingRequestDetail { Id = 1, BookId = 1 },
            new BookBorrowingRequestDetail { Id = 2, BookId = 2 }
        };

            // Use Moq.EntityFrameworkCore to mock DbSet
            _contextMock.Setup(c => c.Set<BookBorrowingRequestDetail>()).ReturnsDbSet(_data);
            _repository = new BookBorrowingRequestDetailRepository(_contextMock.Object);
        }

        [Test]
        public async Task GetAllAsync_WithFilter_ReturnsFilteredList()
        {
            // Arrange
            Expression<Func<BookBorrowingRequestDetail, bool>> where = x => x.BookId == 1;
            Expression<Func<BookBorrowingRequestDetail, BookBorrowingRequestDetail>> select = x => x;

            // Act
            var result = await _repository.GetAllAsync(select, where);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].BookId, Is.EqualTo(1u));
            });
        }

        [Test]
        public async Task GetListAsync_WithPagination_ReturnsPaginatedList()
        {
            // Arrange
            var queryOptions = new QueryOptions<BookBorrowingRequestDetail>
            {
                PageIndex = 1,
                PageSize = 1,
                OrderBy = x => x.Id
            };
            Expression<Func<BookBorrowingRequestDetail, BookBorrowingRequestDetail>> select = x => x;

            // Act
            var result = await _repository.GetListAsync(queryOptions, select);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Items, Has.Count.EqualTo(1));
                Assert.That(result.TotalCount, Is.EqualTo(2));
                Assert.That(result.PageIndex, Is.EqualTo(1));
                Assert.That(result.PageSize, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsEntity()
        {
            // Arrange
            var id = 1u;

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(id));
                Assert.That(result.BookId, Is.EqualTo(1u));
            });
        }

        [Test]
        public async Task AddAsync_ValidEntity_AddsToContext()
        {
            // Arrange
            var entity = new BookBorrowingRequestDetail { Id = 3, BookId = 3 };

            // Act
            await _repository.AddAsync(entity);

            // Assert
            _contextMock.Verify(c => c.Set<BookBorrowingRequestDetail>().AddAsync(entity, default), Times.Once());
        }

        [Test]
        public void Update_ValidEntity_UpdatesContext()
        {
            // Arrange
            var entity = new BookBorrowingRequestDetail { Id = 1, BookId = 1 };

            // Act
            _repository.Update(entity);

            // Assert
            _contextMock.Verify(c => c.Set<BookBorrowingRequestDetail>().Update(entity), Times.Once());
        }

        [Test]
        public void Delete_ValidEntity_RemovesFromContext()
        {
            // Arrange
            var entity = new BookBorrowingRequestDetail { Id = 1, BookId = 1 };

            // Act
            _repository.Delete(entity);

            // Assert
            _contextMock.Verify(c => c.Set<BookBorrowingRequestDetail>().Remove(entity), Times.Once());
        }
    }
}