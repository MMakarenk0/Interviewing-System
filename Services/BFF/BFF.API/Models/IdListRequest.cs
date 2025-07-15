namespace BFF.API.Models;

public class IdListRequest
{
    public List<Guid> Ids { get; set; } = new();
}