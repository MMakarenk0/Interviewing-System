using System.ComponentModel.DataAnnotations;

namespace InterviewService.BLL.Models;

public class CreateAssessmentDto
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Guid InterviewerId { get; set; }

    [Range(1, 10)]
    public int TechnicalScore { get; set; }

    [Range(1, 10)]
    public int CommunicationScore { get; set; }

    [Required]
    public string Feedback { get; set; }
}

