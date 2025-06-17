namespace DAL_Core.Entities;

public class Resume : BaseEntity
{
    public string FileName { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<Application> Applications { get; set; }
}

