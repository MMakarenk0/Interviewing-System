using Microsoft.AspNetCore.Identity;

namespace DAL_Core.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<Resume> Resumes { get; set; }
    public ICollection<InterviewSlot> InterviewSlots { get; set; } // as interviewer
    public ICollection<Assessment> Assessments { get; set; } // as interviewer
    public CandidateProfile CandidateProfile { get; set; } // if candidate
}



