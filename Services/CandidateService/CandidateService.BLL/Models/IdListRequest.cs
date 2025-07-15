namespace CandidateService.API.Controllers;

public class IdListRequest
{
    public List<Guid> Ids { get; set; } = new();
}