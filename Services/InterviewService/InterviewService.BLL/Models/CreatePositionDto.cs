namespace InterviewService.BLL.Models;

public class CreatePositionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}