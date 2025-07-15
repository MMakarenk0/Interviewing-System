using Microsoft.AspNetCore.Http;

namespace CandidateService.BLL.Models;
public class CreateApplicationDto
{
    public Guid Id { get; set; }
    public Guid CandidateProfileId { get; set; }
    public Guid PositionId { get; set; }
    public IFormFile? ResumeFile { get; set; }
}

