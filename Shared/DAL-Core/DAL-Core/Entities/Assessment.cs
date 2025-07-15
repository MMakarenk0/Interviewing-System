namespace DAL_Core.Entities;

public class Assessment : BaseEntity
{
    public int TechnicalScore { get; set; }
    public int CommunicationScore { get; set; }
    public string Feedback { get; set; }

    public Guid InterviewerId { get; set; }

    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; }
}