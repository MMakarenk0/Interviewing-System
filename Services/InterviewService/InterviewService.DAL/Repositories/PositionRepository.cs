using DAL_Core;
using DAL_Core.Entities;
using InterviewService.DAL.Repositories.Interfaces;

namespace InterviewService.DAL.Repositories;
public class PositionRepository : Repository<Position>, IPositionRepository
{
    private readonly InterviewingSystemDbContext _context;
    public PositionRepository(InterviewingSystemDbContext context) : base(context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IQueryable<Position> GetAll()
    {
        return _context.Positions.AsQueryable();
    }
}

