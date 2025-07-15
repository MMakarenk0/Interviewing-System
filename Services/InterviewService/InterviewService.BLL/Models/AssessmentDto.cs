namespace InterviewService.BLL.Models;

public class AssessmentDto
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }

    public int TechnicalScore { get; set; }
    public int CommunicationScore { get; set; }
    public string Feedback { get; set; }

    public string InterviewerFullName { get; set; }
    public DateTime InterviewDate { get; set; }
}

