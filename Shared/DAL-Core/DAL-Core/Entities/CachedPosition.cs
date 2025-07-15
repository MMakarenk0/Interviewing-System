namespace DAL_Core.Entities;

public class CachedPosition : BaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}

