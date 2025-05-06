namespace Contract.Dtos.Books.Responses;

public class GetAllBooksResponseDto
{
    public uint Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public uint CategoryId { get; set; }
    public uint Quantity { get; set; }
    public uint Available { get; set; }
    public string? Category { get; set; }
    public DateOnly PublicationDate { get; set; }
}