using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;

namespace InterviewService.DAL.Repositories;
public class InterviewSlotRepository : Repository<InterviewSlot>, IInterviewSlotRepository
{
    private readonly InterviewingSystemDbContext _context;
    public InterviewSlotRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
}


