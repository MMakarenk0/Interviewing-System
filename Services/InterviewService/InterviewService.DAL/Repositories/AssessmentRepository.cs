using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.DAL.Repositories;
public class AssessmentRepository : Repository<Assessment>, IAssessmentRepository
{
    private readonly InterviewingSystemDbContext _context;
    public AssessmentRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Assessment?> GetWithInterviewAsync(Guid id)
    {
        return await _context.Assessments
            .Include(a => a.Interview)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}

