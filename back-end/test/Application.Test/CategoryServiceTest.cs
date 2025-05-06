using System.Linq.Expressions;
using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Dtos.Common;
using Contract.Repositories;
using Contract.Shared;
using Contract.UnitOfWork;
using Domain.Entities;
using Moq;

namespace Application.Test
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private CategoryService _service;
        private List<Category> _categories;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _categories = new List<Category>();

            _unitOfWorkMock.SetupGet(u => u.CategoryRepository).Returns(Mock.Of<ICategoryRepository>());
            _unitOfWorkMock.SetupGet(u => u.BookRepository).Returns(Mock.Of<IBookRepository>());

            _service = new CategoryService(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task GetAllCategories_ValidRequest_ReturnsCategoryList()
        {
            // Arrange
            var categories = new List<GetCategoryResponseDto>
            {
                new GetCategoryResponseDto(1, "Fiction"),
                new GetCategoryResponseDto(2, "Non-Fiction")
            };

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetAllAsync(It.IsAny<Expression<Func<Category, GetCategoryResponseDto>>>(), null))
                .ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllCategories();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Has.Count.EqualTo(2));
                Assert.That(result.Value![0].Id, Is.EqualTo(1));
                Assert.That(result.Value[0].Name, Is.EqualTo("Fiction"));
                Assert.That(result.Value[1].Id, Is.EqualTo(2));
                Assert.That(result.Value[1].Name, Is.EqualTo("Non-Fiction"));
            });
        }

        [Test]
        public async Task GetListCategoriesFilterAsync_ValidRequest_ReturnsPaginatedList()
        {
            // Arrange
            var request = new GetAllCategoryFilterRequestDto
            {
                Name = "Fic",
                PageIndex = 1,
                PageSize = 2
            };
            var paginatedList = new PaginatedList<GetCategoryResponseDto>(
                new List<GetCategoryResponseDto>
                {
                    new GetCategoryResponseDto(1, "Fiction")
                }, 1, 1, 2);

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetListAsync(
                It.IsAny<QueryOptions<Category>>(),
                It.IsAny<Expression<Func<Category, GetCategoryResponseDto>>>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _service.GetListCategoriesFilterAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Is.Not.Null);
                Assert.That(result.Value!.Items, Has.Count.EqualTo(1));
                Assert.That(result.Value.Items[0].Name, Is.EqualTo("Fiction"));
                Assert.That(result.Value.TotalCount, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task AddNewCategoryAsync_ValidRequest_AddsCategory()
        {
            // Arrange
            var request = new AddCategoryRequestDto(Name: "Science");
            var category = new Category { Id = 0, Name = "Science" }; // ID set to 0 initially

            _unitOfWorkMock.Setup(u => u.CategoryRepository.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c =>
                {
                    c.Id = 1; // Simulate database ID assignment
                    _categories.Add(c);
                });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddNewCategoryAsync(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Is.EqualTo(1u));
                Assert.That(_categories, Has.Count.EqualTo(1));
                Assert.That(_categories[0].Id, Is.EqualTo(1));
                Assert.That(_categories[0].Name, Is.EqualTo("Science"));
            });
        }

        [Test]
        public async Task UpdateCategoryAsync_ValidRequest_UpdatesCategory()
        {
            // Arrange
            const uint id = 1u;
            var request = new UpdateCategoryRequestDto(Name: "Updated Fiction");
            var category = new Category { Id = 1, Name = "Fiction" };

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(id, null))
                .ReturnsAsync(category);
            _unitOfWorkMock.Setup(u => u.CategoryRepository.Update(It.IsAny<Category>()))
                .Callback<Category>(c => _categories.Add(c));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateCategoryAsync(id, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(_categories, Has.Count.EqualTo(1));
                Assert.That(_categories[0].Name, Is.EqualTo("Updated Fiction"));
            });
        }

        [Test]
        public async Task UpdateCategoryAsync_NonExistentCategory_ReturnsFailure()
        {
            // Arrange
            const uint id = 1u;
            var request = new UpdateCategoryRequestDto(Name: "Updated Fiction");

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(id, null))
                .ReturnsAsync((Category)null!);

            // Act
            var result = await _service.UpdateCategoryAsync(id, request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo("Category not found"));
                Assert.That(_categories, Is.Empty);
            });
        }

        [Test]
        public async Task DeleteCategoryAsync_ValidRequest_DeletesCategory()
        {
            // Arrange
            var id = 1u;
            var category = new Category { Id = 1, Name = "Fiction" };

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(id,null))
                .ReturnsAsync(category);
            _unitOfWorkMock.Setup(u => u.BookRepository.IsInCategoryAsync(id))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.CategoryRepository.Delete(It.IsAny<Category>()))
                .Callback<Category>(c => _categories.Remove(c));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteCategoryAsync(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Is.True);
                Assert.That(_categories, Is.Empty);
            });
        }

        [Test]
        public async Task DeleteCategoryAsync_NonExistentCategory_ReturnsFailure()
        {
            // Arrange
            var id = 1u;

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(id, null))
                .ReturnsAsync((Category)null!);

            // Act
            var result = await _service.DeleteCategoryAsync(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo("Category not found!"));
                Assert.That(_categories, Is.Empty);
            });
        }

        [Test]
        public async Task DeleteCategoryAsync_CategoryWithBooks_ReturnsFailure()
        {
            // Arrange
            var id = 1u;
            var category = new Category { Id = 1, Name = "Fiction" };

            _unitOfWorkMock.Setup(u => u.CategoryRepository.GetByIdAsync(id, null))
                .ReturnsAsync(category);
            _unitOfWorkMock.Setup(u => u.BookRepository.IsInCategoryAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCategoryAsync(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo("Cannot delete category because it contains books!"));
                Assert.That(_categories, Is.Empty);
            });
        }
    }
}