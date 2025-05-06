using Domain.Entities;

namespace Contract.Dtos.Books.Requests;

public class AddBookRequestDto
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public DateOnly PublicationDate { get; set; }
    public uint Quantity { get; set; }
    public uint CategoryId { get; set; }

    public Book ToBook()
    {
        return new Book
        {
            Title = Title,
            Author = Author,
            ISBN = ISBN,
            PublicationDate = PublicationDate,
            Quantity = Quantity,
            Available = Quantity,
            CategoryId = CategoryId,
        };
    }
}
