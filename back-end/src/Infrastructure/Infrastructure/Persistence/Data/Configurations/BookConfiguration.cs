using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(book => book.Id);

        builder.Property(book => book.ISBN)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(book => book.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(book => book.Category)
            .WithMany(category => category.Books)
            .HasForeignKey(book => book.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(
            new Book
            {
                Id = 1,
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                ISBN = "978-0446310789",
                PublicationDate = new DateOnly(1960, 7, 11),
                Quantity = 5,
                Available = 3,
                CategoryId = 1,
                IsDeleted = false
            }, new Book
            {
                Id = 2,
                Title = "Pride and Prejudice",
                Author = "Jane Austen",
                ISBN = "978-0141439518",
                PublicationDate = new DateOnly(1813, 1, 28),
                Quantity = 4,
                Available = 2,
                CategoryId = 1,
                IsDeleted = false
            }, new Book
            {
                Id = 3,
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                ISBN = "978-0743273565",
                PublicationDate = new DateOnly(1925, 4, 10),
                Quantity = 6,
                Available = 4,
                CategoryId = 1,
                IsDeleted = false
            }, new Book
            {
                Id = 4,
                Title = "1984",
                Author = "George Orwell",
                ISBN = "978-0451524935",
                PublicationDate = new DateOnly(1949, 6, 8),
                Quantity = 3,
                Available = 0,
                CategoryId = 1,
                IsDeleted = false
            }, new Book
            {
                Id = 5,
                Title = "The Catcher in the Rye",
                Author = "J.D. Salinger",
                ISBN = "978-0316769488",
                PublicationDate = new DateOnly(1951, 7, 16),
                Quantity = 2,
                Available = 1,
                CategoryId = 1,
                IsDeleted = true
            }, new Book
            {
                Id = 6,
                Title = "Sapiens: A Brief History of Humankind",
                Author = "Yuval Noah Harari",
                ISBN = "978-0062316097",
                PublicationDate = new DateOnly(2014, 9, 1),
                Quantity = 7,
                Available = 5,
                CategoryId = 2,
                IsDeleted = false
            }, new Book
            {
                Id = 7,
                Title = "Educated",
                Author = "Tara Westover",
                ISBN = "978-0399590504",
                PublicationDate = new DateOnly(2018, 2, 20),
                Quantity = 4,
                Available = 3,
                CategoryId = 2,
                IsDeleted = false
            }, new Book
            {
                Id = 8,
                Title = "The Immortal Life of Henrietta Lacks",
                Author = "Rebecca Skloot",
                ISBN = "978-1400052189",
                PublicationDate = new DateOnly(2010, 2, 2),
                Quantity = 3,
                Available = 2,
                CategoryId = 2,
                IsDeleted = false
            }, new Book
            {
                Id = 9,
                Title = "Thinking, Fast and Slow",
                Author = "Daniel Kahneman",
                ISBN = "978-0374533557",
                PublicationDate = new DateOnly(2011, 10, 25),
                Quantity = 5,
                Available = 4,
                CategoryId = 2,
                IsDeleted = false
            }, new Book
            {
                Id = 10,
                Title = "The Power of Habit",
                Author = "Charles Duhigg",
                ISBN = "978-0812981605",
                PublicationDate = new DateOnly(2012, 2, 28),
                Quantity = 2,
                Available = 0,
                CategoryId = 2,
                IsDeleted = true
            }, new Book
            {
                Id = 11,
                Title = "Dune",
                Author = "Frank Herbert",
                ISBN = "978-0441013593",
                PublicationDate = new DateOnly(1965, 8, 1),
                Quantity = 6,
                Available = 5,
                CategoryId = 3,
                IsDeleted = false
            }, new Book
            {
                Id = 12,
                Title = "The Martian",
                Author = "Andy Weir",
                ISBN = "978-0804139021",
                PublicationDate = new DateOnly(2014, 2, 11),
                Quantity = 4,
                Available = 3,
                CategoryId = 3,
                IsDeleted = false
            }, new Book
            {
                Id = 13,
                Title = "Neuromancer",
                Author = "William Gibson",
                ISBN = "978-0441569595",
                PublicationDate = new DateOnly(1984, 7, 1),
                Quantity = 3,
                Available = 2,
                CategoryId = 3,
                IsDeleted = false
            }, new Book
            {
                Id = 14,
                Title = "Foundation",
                Author = "Isaac Asimov",
                ISBN = "978-0553293357",
                PublicationDate = new DateOnly(1951, 5, 1),
                Quantity = 5,
                Available = 4,
                CategoryId = 3,
                IsDeleted = false
            }, new Book
            {
                Id = 15,
                Title = "Snow Crash",
                Author = "Neal Stephenson",
                ISBN = "978-0553380958",
                PublicationDate = new DateOnly(1992, 6, 1),
                Quantity = 2,
                Available = 1,
                CategoryId = 3,
                IsDeleted = false
            }, new Book
            {
                Id = 16,
                Title = "Steve Jobs",
                Author = "Walter Isaacson",
                ISBN = "978-1451648539",
                PublicationDate = new DateOnly(2011, 10, 24),
                Quantity = 4,
                Available = 3,
                CategoryId = 4,
                IsDeleted = false
            }, new Book
            {
                Id = 17,
                Title = "The Diary of a Young Girl",
                Author = "Anne Frank",
                ISBN = "978-0553577129",
                PublicationDate = new DateOnly(1947, 6, 25),
                Quantity = 5,
                Available = 4,
                CategoryId = 4,
                IsDeleted = false
            }, new Book
            {
                Id = 18,
                Title = "Becoming",
                Author = "Michelle Obama",
                ISBN = "978-1524763138",
                PublicationDate = new DateOnly(2018, 11, 13),
                Quantity = 6,
                Available = 5,
                CategoryId = 4,
                IsDeleted = false
            }
        );
    }
}