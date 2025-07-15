using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;

namespace InterviewService.DAL.Repositories;

public class CachedApplicationRepository : Repository<CachedApplication>, ICachedApplicationRepository
{
    private readonly InterviewingSystemDbContext _context;
    public CachedApplicationRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
}

