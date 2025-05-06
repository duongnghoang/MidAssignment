using Domain.Entities;

namespace Contract.Dtos.Books.Requests;

public class UpdateBookRequestDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public DateOnly PublicationDate { get; set; }
    public uint Quantity { get; set; }
    public uint Available { get; set; }
    public uint CategoryId { get; set; }

    public void ToBook(Book existingBook)
    {
        existingBook.Title = Title;
        existingBook.Author = Author;
        existingBook.ISBN = ISBN;
        existingBook.PublicationDate = PublicationDate;
        existingBook.Quantity = Quantity;
        existingBook.Available = Available;
        existingBook.CategoryId = CategoryId;
    }
}