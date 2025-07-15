using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.DAL.Repositories;
public class InterviewRepository : Repository<Interview>, IInterviewRepository
{
    private readonly InterviewingSystemDbContext _context;
    public InterviewRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Interview?> GetWithSlotAsync(Guid id)
    {
        return await _context.Interviews
            .Include(i => i.Slot)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<ICollection<Interview>> GetAllWithSlotAsync()
    {
        return await _context.Interviews
            .Include(i => i.Slot)
            .ToListAsync();
    }
}