namespace Domain.Abstractions.Base;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
}